using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Types.Descriptors;
using Microsoft.EntityFrameworkCore;
using sport_sync.GraphQL;
using sport_sync.GraphQL.Mutations;
using sport_sync.GraphQL.Queries;
using sport_sync.Setup;
using SportSync.Application;
using SportSync.Infrastructure;
using SportSync.Persistence;

namespace SportSync.Api;


public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddHttpContextAccessor()
            .AddApplication()
            .AddInfrastructure(builder.Configuration)
            .AddPersistence(builder.Configuration);

        builder.Services.AddCors(x => x.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
        }));

        builder.Services.AddHttpResponseFormatter<CustomHttpResponseFormatter>();

        builder.Services
            .AddGraphQLServer()
            .AddErrorFilter<GraphQlErrorFilter>()
            .AddProjections()
            .AddFiltering()
            .AddAuthorization()
            .AddFluentValidation(x => x.UseErrorMapper((errorBuilder, context) =>
            {
                errorBuilder.SetMessage(context.ValidationFailure.ErrorMessage);
            }))
            .AddQueryType(q => q.Name("Query"))
            .AddType<UploadType>()
            .AddType<UserQuery>()
            .AddType<NotificationQuery>()
            .AddType<EventQuery>()
            .AddType<MatchQuery>()
            .AddType<FriendshipQuery>()
            .AddMutationType(q => q.Name("Mutation"))
            .AddType<UserMutation>()
            .AddType<NotificationMutation>()
            .AddType<MatchMutation>()
            .AddType<EventMutation>()
            .AddType<TeamMutation>()
            .AddType<FriendshipMutation>()
            .AddTypeConverter<DateTime, DateTimeOffset>(t => t.Kind is DateTimeKind.Unspecified ? DateTime.SpecifyKind(t, DateTimeKind.Utc) : t)
            .AddTypeConverter<DateTimeOffset, DateTime>(d => d.DateTime)
            .AddConvention<INamingConventions>(new EnumNamingConvention());

        //.AddSubscriptionType<Subscription>()
        //.RegisterService<CreateUserRequestHandler>();
        //.AddGlobalObjectIdentification();

        Console.WriteLine($"Connection string: {builder.Configuration.GetConnectionString("SportSyncDb")}");

        builder.Host.SetupSerilog(builder.Configuration.GetConnectionString("SportSyncDb"));

        var app = builder.Build();

        using IServiceScope serviceScope = app.Services.CreateScope();
        await using SportSyncDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<SportSyncDbContext>();
        await dbContext.Database.MigrateAsync();

        app.UseCors();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseWebSockets();
        app.MapGraphQL("/api");

        app.Run();
    }
}
