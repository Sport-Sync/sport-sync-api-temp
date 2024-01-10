using AppAny.HotChocolate.FluentValidation;
using Microsoft.EntityFrameworkCore;
using sport_sync.GraphQL;
using SportSync.Application;
using SportSync.GraphQL;
using SportSync.Infrastructure;
using SportSync.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.AddErrorFilter<GraphQLErrorFilter>();

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

//app.UseCustomExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.MapGraphQL("/api");

app.Run();
