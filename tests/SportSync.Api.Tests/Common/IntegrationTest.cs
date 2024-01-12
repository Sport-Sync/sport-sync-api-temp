using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.GraphQL;

namespace SportSync.Api.Tests.Common;

public class IntegrationTest
{
    private RequestExecutorProxy _executor { get; }

    public IServiceProvider ServiceProvider { get; set; }
    public Mock<IUserIdentifierProvider> UserIdentifierMock { get; set; }
    public Database Database { get; }

    public IntegrationTest()
    {
        Database = Database.Create();

        UserIdentifierMock = new Mock<IUserIdentifierProvider>();
        UserIdentifierMock.Setup(x => x.UserId).Returns(Guid.NewGuid);

        ServiceProvider = new ServiceCollection()
            .AddGraphQLServer()
            .AddAuthorizationHandler<IntegrationTestAuthorizationHandler>()
            .AddProjections()
            .AddFiltering()
            .AddQueryType<Query>()
            .Services
                .AddSingleton(
                    sp =>
                        new RequestExecutorProxy(
                            sp.GetRequiredService<IRequestExecutorResolver>(),
                            Schema.DefaultName))
                .AddLogging()
                .AddScoped(_ => Database.UnitOfWork)
                .AddScoped(_ => Database.DbContext)
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

        return result.ToJson();
    }
}