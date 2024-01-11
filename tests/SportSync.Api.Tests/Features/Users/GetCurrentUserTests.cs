using FluentAssertions;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Entities;
using SportSync.GraphQL;
using SportSync.Persistence;

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

        var result = await TestServices.ExecuteRequestAsync(
            q => q.SetQuery(@"query{
                me{
                    firstName
                }
            }"));

        var returnedUser = JsonConvert.DeserializeObject<QueryResponse>(result);
        dynamic meData = returnedUser.Data.me;
        string firstName = meData.firstName;
        firstName.Should().Be("Ante");
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
        CancellationToken cancellationToken = default)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();

        var requestBuilder = new QueryRequestBuilder();
        requestBuilder.SetServices(scope.ServiceProvider);
        configureRequest(requestBuilder);

        var request = requestBuilder.Create();

        await using var result = await Executor.ExecuteAsync(request, cancellationToken);

        result.ExpectQueryResult();

        return result.ToJson();
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

        ServiceProvider = new ServiceCollection()
            .AddGraphQLServer()
            .AddProjections()
            .AddFiltering()
            .AddAuthorization()
            .AddQueryType<Query>()
            .Services
            .AddSingleton(
                sp =>
                    new RequestExecutorProxy(
                        sp.GetRequiredService<IRequestExecutorResolver>(),
                        Schema.DefaultName))
            .AddScoped<IUnitOfWork>(_ => dbContext)
            .AddScoped<IDbContext>(_ => dbContext)
            .AddScoped<IUserIdentifierProvider>(_ => UserIdentifierMock.Object)
            .BuildServiceProvider();

        Executor = ServiceProvider.GetRequiredService<RequestExecutorProxy>();
    }

}

public class TestLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        throw new NotImplementedException();
    }
}