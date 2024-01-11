using AppAny.HotChocolate.FluentValidation;
using Microsoft.EntityFrameworkCore;
using sport_sync.GraphQL;
using SportSync.Application;
using SportSync.GraphQL;
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

        builder.Services.AddErrorFilter<GraphQlErrorFilter>();

        builder.Services.AddCors(x => x.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
        }));

        builder.Services
            .AddGraphQLServer()
            .AddProjections()
            .AddFiltering()
            .AddAuthorization()
            .AddFluentValidation(x => x.UseErrorMapper((errorBuilder, context) =>
            {
                errorBuilder.SetMessage(context.ValidationFailure.ErrorMessage);
            }))
            .AddQueryType<Query>()
            .AddMutationType<Mutation>();
        //.AddSubscriptionType<Subscription>()
        //.RegisterService<CreateUserRequestHandler>();
        //.AddGlobalObjectIdentification();

        var app = builder.Build();

        using IServiceScope serviceScope = app.Services.CreateScope();
        using SportSyncDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<SportSyncDbContext>();
        dbContext.Database.Migrate();

        app.UseCors();

        //app.UseCustomExceptionHandler();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseWebSockets();
        app.MapGraphQL("/api");

        app.Run();
    }
}