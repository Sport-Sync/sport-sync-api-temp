using System.IdentityModel.Tokens.Jwt;
using System;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using FluentAssertions;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Execution;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.GraphQL;
using SportSync.Infrastructure.Authentication.Settings;
using SportSync.Persistence;
using User = SportSync.Domain.Entities.User;

namespace SportSync.Api.Tests.Features.Users;

public class GetCurrentUserTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Query_Me_ShouldReturnCurrentUser()
    {
        var user = User.Create("Ante", "Kadić", "ante.kadic@gmail.com", "092374342", "98637286423");
        TestServices.DbContext.Insert(user);
        await TestServices.UnitOfWork.SaveChangesAsync();
        TestServices.UserIdentifierMock.Setup(x => x.UserId).Returns(user.Id);

        var token = CreateToken(user);

        var result = await TestServices.ExecuteRequestAsync(
            q => q.SetQuery(@"query{
                me{
                    firstName
                }
            }"), user);

        var returnedUser = JsonConvert.DeserializeObject<QueryResponse>(result);
        dynamic meData = returnedUser.Data.me;
        string firstName = meData.firstName;
        firstName.Should().Be("Ante");
    }

    public string CreateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("iwgf4378b389c4tz7843tbvzc38tf83btc874tzr873bvc93o4bcz4r39o"));

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        {
            new ("userId", user.Id.ToString()),
            new ("email", user.Email),
            new ("phone", user.Phone),
            new ("name", user.FullName)
        };

        var tokenExpirationTime = DateTime.UtcNow.AddDays(1);

        var token = new JwtSecurityToken(
            "sport-sync-api",
            "sport-sync-mobile-app",
            claims,
            null,
            tokenExpirationTime,
            signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}

public class QueryResponse
{
    public dynamic Data { get; set; }
}

public static class TestServices
{
    public static IServiceProvider ServiceProvider { get; set; }
    public static RequestExecutorProxy Executor { get; set; }
    public static Mock<IUserIdentifierProvider> UserIdentifierMock { get; set; }
    public static IDbContext DbContext { get; set; }
    public static IUnitOfWork UnitOfWork { get; set; }
    
    public static async Task<string> ExecuteRequestAsync(
        Action<IQueryRequestBuilder> configureRequest,
        User user = null,
        CancellationToken cancellationToken = default)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();

        var requestBuilder = new QueryRequestBuilder();
        requestBuilder.SetServices(scope.ServiceProvider);
        configureRequest(requestBuilder);

        var request = requestBuilder
            //.SetUser(CreatePrincipal(user))
            //.AddGlobalState(nameof(ClaimsPrincipal), CreatePrincipal(user))
            .Create();

        await using var result = await Executor.ExecuteAsync(request, cancellationToken);

        result.ExpectQueryResult();

        return result.ToJson();
    }

    public static ClaimsPrincipal CreatePrincipal(User user)
    {
        //id ??= Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(new ClaimsIdentity(new[] { new Claim("userId", user.Id.ToString()) }));
        claimsPrincipal.AddIdentity(new ClaimsIdentity(new[] { new Claim("email", user.Email.ToString()) }));
        claimsPrincipal.AddIdentity(new ClaimsIdentity(new[] { new Claim("phone", user.Phone.ToString()) }));
        claimsPrincipal.AddIdentity(new ClaimsIdentity(new[] { new Claim("name", user.FullName.ToString()) }));

        return claimsPrincipal;
    }

    private static SportSyncDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SportSyncDbContext>()
            .UseInMemoryDatabase("sport-sync")
            .Options;

        var dateTimeMock = new Mock<IDateTime>();

        var dbContext = new SportSyncDbContext(options, dateTimeMock.Object);
        DbContext = dbContext;
        UnitOfWork = dbContext;

        return dbContext;
    }

    static TestServices()
    {
        var dbContext = CreateDbContext();

        UserIdentifierMock = new Mock<IUserIdentifierProvider>();
        UserIdentifierMock.Setup(x => x.UserId).Returns(Guid.NewGuid);

        MockAuthUser _user = new MockAuthUser(
            new Claim("userId", Guid.NewGuid().ToString()),
            new Claim("sub", Guid.NewGuid().ToString()),
            new Claim("email", "default-user@xyz.com"));

        ServiceProvider = new ServiceCollection()
            //.AddAuthentication()
            //.AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidateAudience = true,
            //    ValidateLifetime = true,
            //    ValidateIssuerSigningKey = true,
            //    ValidIssuer = "sport-sync-api",
            //    ValidAudience = "sport-sync-mobile-app",
            //    IssuerSigningKey = new SymmetricSecurityKey(
            //        Encoding.UTF8.GetBytes("iwgf4378b389c4tz7843tbvzc38tf83btc874tzr873bvc93o4bcz4r39o"))
            //})
            //.Services
            .AddGraphQLServer()
            .AddAuthorizationHandler<CustomAuthorizationHandler>()
            .AddProjections()
            .AddFiltering()
            //.AddAuthorization()
            .AddQueryType<Query>()
        .Services
            .AddSingleton(
                sp =>
                    new RequestExecutorProxy(
                        sp.GetRequiredService<IRequestExecutorResolver>(),
                        Schema.DefaultName))
            .AddLogging()
            .AddScoped<IUnitOfWork>(_ => dbContext)
            .AddScoped<IDbContext>(_ => dbContext)
            .AddScoped<IUserIdentifierProvider>(_ => UserIdentifierMock.Object)
            .BuildServiceProvider();

        Executor = ServiceProvider.GetRequiredService<RequestExecutorProxy>();
    }

}

public class CustomAuthorizationHandler : HotChocolate.Authorization.IAuthorizationHandler
{
    public ValueTask<AuthorizeResult> AuthorizeAsync(IMiddlewareContext context, AuthorizeDirective directive,
        CancellationToken cancellationToken = new CancellationToken())
    {
        //var claimsPrincipal = new ClaimsPrincipal();
        //claimsPrincipal.AddIdentity(new ClaimsIdentity(new[] { new Claim("userId", user.Id.ToString()) }));
        //claimsPrincipal.AddIdentity(new ClaimsIdentity(new[] { new Claim("email", "pero") }));
        //claimsPrincipal.AddIdentity(new ClaimsIdentity(new[] { new Claim("phone", "perić") }));
        //claimsPrincipal.AddIdentity(new ClaimsIdentity(new[] { new Claim("name", "wehfiewrn") }));
        //return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, authenticationScheme: "Bearer"));
        return ValueTask.FromResult(AuthorizeResult.Allowed);
    }

    public ValueTask<AuthorizeResult> AuthorizeAsync(AuthorizationContext context, IReadOnlyList<AuthorizeDirective> directives,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return ValueTask.FromResult(AuthorizeResult.Allowed);
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly MockAuthUser _mockAuthUser;

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        MockAuthUser mockAuthUser)
        : base(options, logger, encoder, clock)
    {
        // 1. We get a "mock" user instance here via DI.
        // we'll see how this work later, don't worry
        _mockAuthUser = mockAuthUser;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (_mockAuthUser.Claims.Count == 0)
            return Task.FromResult(AuthenticateResult.Fail("Mock auth user not configured."));

        // 2. Create the principal and the ticket
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Bearer");

        // 3. Authenticate the request
        var result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }
}

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddTestAuthentication(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // AuthConstants.Scheme is just a scheme we define. I called it "TestAuth"
            options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        // Register our custom authentication handler
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                JwtBearerDefaults.AuthenticationScheme, options => { });

        return services;
    }
}

public class MockAuthUser
{
    public List<Claim> Claims { get; private set; } = new();

    public MockAuthUser(params Claim[] claims)
        => Claims = claims.ToList();
}