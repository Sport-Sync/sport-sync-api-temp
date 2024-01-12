using System.Reflection;
using AppAny.HotChocolate.FluentValidation;
using FluentValidation;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using sport_sync.GraphQL;
using sport_sync.GraphQL.Types;
using SportSync.Application;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Users.CreateUser;
using SportSync.Infrastructure;
using SportSync.Infrastructure.Authentication.Settings;
using SportSync.Persistence;

namespace SportSync.Api.Tests.Common;

public class IntegrationTest : IDisposable
{
    private RequestExecutorProxy _executor { get; }

    public IServiceProvider ServiceProvider { get; set; }
    public Mock<IUserIdentifierProvider> UserIdentifierMock { get; set; }
    public Database Database { get; private set; }

    public IntegrationTest()
    {
        Database = Database.Create();

        UserIdentifierMock = new Mock<IUserIdentifierProvider>();
        UserIdentifierMock.Setup(x => x.UserId).Returns(Guid.NewGuid);

        ServiceProvider = new ServiceCollection()
            .AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(CreateUserInputValidator)))
            .RegisterRequestHandlers()
            .RegisterInfrastructureServices()
            .AddRepositories()
            .AddGraphQLServer()
            .AddErrorFilter<GraphQlErrorFilter>()
            .AddAuthorizationHandler<IntegrationTestAuthorizationHandler>()
            .AddProjections()
            .AddFiltering()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddFluentValidation(x => x.UseErrorMapper((errorBuilder, context) =>
            {
                errorBuilder.SetMessage(context.ValidationFailure.ErrorMessage);
            }))
            .Services
                .AddSingleton(
                    sp =>
                        new RequestExecutorProxy(
                            sp.GetRequiredService<IRequestExecutorResolver>(),
                            Schema.DefaultName))
                .AddLogging()
                .AddScoped(_ => Database.UnitOfWork)
                .AddScoped(_ => Database.DbContext)
                .Configure<JwtSettings>(x =>
                    {
                        x.SecurityKey = "12343tr34tt34t35t53";
                        x.TokenExpirationInDays = 1;
                    })
                .AddScoped(_ => UserIdentifierMock.Object)
                .BuildServiceProvider();

        _executor = ServiceProvider.GetRequiredService<RequestExecutorProxy>();
    }

    public async Task<string> ExecuteRequestAsync(
        Action<IQueryRequestBuilder> configureRequest,
        CancellationToken cancellationToken = default)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();

        var requestBuilder = new QueryRequestBuilder();
        requestBuilder.SetServices(scope.ServiceProvider);
        configureRequest(requestBuilder);

        var request = requestBuilder.Create();

        await using var result = await _executor.ExecuteAsync(request, cancellationToken);

        Database = Database.Create();

        return result.ToJson();
    }

    public void Dispose()
    {
        _executor?.Dispose();
        (Database.DbContext as SportSyncDbContext).Database.EnsureDeleted();
        (Database.DbContext as SportSyncDbContext).Dispose();
    }
}