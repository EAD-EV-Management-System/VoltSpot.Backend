using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Add application services to the service collection
        /// </summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR for CQRS
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Register FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register AutoMapper
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
