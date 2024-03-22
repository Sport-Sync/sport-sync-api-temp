using System.Reflection;
using AppAny.HotChocolate.FluentValidation;
using FluentValidation;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using sport_sync.GraphQL;
using sport_sync.GraphQL.Mutations;
using sport_sync.GraphQL.Queries;
using SportSync.Application;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Settings;
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
    public Mock<IDateTime> DateTimeProviderMock { get; set; }
    public Database Database { get; private set; }

    public IntegrationTest()
    {
        UserIdentifierMock = new Mock<IUserIdentifierProvider>();
        DateTimeProviderMock = new Mock<IDateTime>();
        UserIdentifierMock.Setup(x => x.UserId).Returns(Guid.NewGuid);

        ServiceProvider = new ServiceCollection()
            .AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(CreateUserRequestValidator)))
            .RegisterRequestHandlers()
            .RegisterInfrastructureServices()
            .AddRepositories()
            .AddGraphQLServer()
            .AddErrorFilter<GraphQlErrorFilter>()
            .AddAuthorizationHandler<IntegrationTestAuthorizationHandler>()
            .AddProjections()
            .AddFiltering()
            .AddQueryType(q => q.Name("Query"))
            .AddType<UploadType>()
            .AddType<UserQuery>()
            .AddType<NotificationQuery>()
            .AddType<MatchQuery>()
            .AddType<FriendshipQuery>()
            .AddMutationType(q => q.Name("Mutation"))
            .AddType<MatchMutation>()
            .AddType<UserMutation>()
            .AddType<NotificationMutation>()
            .AddType<EventMutation>()
            .AddType<FriendshipMutation>()
            .AddConvention<INamingConventions>(new EnumNamingConvention())
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
                .AddSingleton(_ => Database.UnitOfWork)
                .AddSingleton(_ => Database.DbContext)
                .Configure<JwtSettings>(x =>
                    {
                        x.SecurityKey = "12343tr34tt34t35t53";
                        x.TokenExpirationInDays = 1;
                    })
                .Configure<EventSettings>(x =>
                {
                    x.NumberOfMatchesToCreateInFuture = 14;
                })
                .AddScoped(_ => UserIdentifierMock.Object)
                .AddScoped(_ => DateTimeProviderMock.Object)
            .AddTypeConverter<DateTime, DateTimeOffset>(t => DateTime.SpecifyKind(t, DateTimeKind.Utc))
            .AddTypeConverter<DateTimeOffset, DateTime>(d => d.DateTime)
            .BuildServiceProvider();

        Database = Database.Create(ServiceProvider.GetRequiredService<IPublisher>());

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

        Database = Database.Create(ServiceProvider.GetRequiredService<IPublisher>());

        return result.ToJson();
    }

    public void Dispose()
    {
        _executor?.Dispose();
        (Database.DbContext as SportSyncDbContext).Database.EnsureDeleted();
        (Database.DbContext as SportSyncDbContext).Dispose();
    }
}