using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using sport_sync.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>();
//.AddGlobalObjectIdentification();

//builder.Services
//    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        var tokenSettings = builder.Configuration
//            .GetSection("JwtSettings").Get<JwtSettings>();
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidIssuer = tokenSettings.Issuer,
//            ValidateIssuer = true,
//            ValidAudience = tokenSettings.Audience,
//            ValidateAudience = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret)),
//            ValidateIssuerSigningKey = true,
//            //ClockSkew = TimeSpan.Zero // enable this line to validate the expiration time below 5mins
//        };
//    });

var app = builder.Build();

app.MapGraphQL();

app.Run();
