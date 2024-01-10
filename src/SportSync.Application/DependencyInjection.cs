using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SportSync.Application.Core.Abstractions.Common;

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

            return services;
        }

        private static void RegisterRequestHandlers(this IServiceCollection services)
        {
            var requestHandlerTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract &&
                            !t.IsInterface &&
                            t.GetInterfaces().Any(i => i.IsGenericType &&
                                                       i.GetGenericTypeDefinition() == typeof(IInputHandler<,>)));

            foreach (var handlerType in requestHandlerTypes)
            {
                services.AddScoped(handlerType);
            }
        }
    }
}
