using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Common;
using SportSync.Application.Core.Abstractions.Cryptography;
using SportSync.Application.Core.Abstractions.Storage;
using SportSync.Application.Core.Settings;
using SportSync.Domain.Services;
using SportSync.Infrastructure.Authentication;
using SportSync.Infrastructure.Authentication.Settings;
using SportSync.Infrastructure.Common;
using SportSync.Infrastructure.Cryptography;
using SportSync.Infrastructure.Jobs.Setup;
using SportSync.Infrastructure.Storage;

namespace SportSync.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"]))
            });

        services.AddQuartz();
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.ConfigureOptions<CreateAdditionalMatchesJobSetup>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SettingsKey));
        services.Configure<EventSettings>(configuration.GetSection(EventSettings.SettingsKey));

        services.RegisterInfrastructureServices();

        //FirebaseApp.Create(new AppOptions()
        //{
        //    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/firebase.json")),
        //});

        return services;
    }

    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services)
    {
        //services.Configure<MailSettings>(configuration.GetSection(MailSettings.SettingsKey));

        //services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));

        services.AddScoped<IUserIdentifierProvider, UserIdentifierProvider>();

        services.AddScoped<IHttpHeaderProvider, HttpHeaderProvider>();

        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddTransient<IDateTime, DateTimeProvider>();

        services.AddTransient<IPasswordHasher, PasswordHasher>();

        services.AddTransient<IPasswordHashChecker, PasswordHasher>();

        services.AddTransient<IBlobStorageService, BlobStorageService>();

        //services.AddTransient<IEmailService, EmailService>();

        //services.AddTransient<IEmailNotificationService, EmailNotificationService>();

        //services.AddSingleton<IIntegrationEventPublisher, IntegrationEventPublisher>();

        return services;
    }
}