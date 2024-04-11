﻿using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SportSync.Application.Core.Services;

namespace SportSync.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers the necessary services with the DI framework.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.RegisterRequestHandlers();
            services.RegisterApplicationServices();

            return services;
        }

        public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            var requestHandlerTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract &&
                            !t.IsInterface &&
                            t.GetInterfaces().Any(i => i.IsGenericType &&
                                                       (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) || 
                                                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))));

            foreach (var handlerType in requestHandlerTypes)
            {
                services.AddScoped(handlerType);
            }

            return services;
        }

        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserProfileImageService, UserProfileImageService>();
            services.AddScoped<IFriendshipInformationService, FriendshipInformationService>();

            return services;
        }
    }
}
