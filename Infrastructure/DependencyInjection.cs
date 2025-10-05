using Application.Interfaces.Services;
using Infrastructure.Common.Models;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Configurations;
using Infrastructure.Data.Context;
using Infrastructure.Data.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using VoltSpot.Domain.Interfaces;
using VoltSpot.Infrastructure.Repositories;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure MongoDB settings
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

            // Configure JWT settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Configure MongoDB conventions
            ConfigureMongoDbConventions();

            // Register MongoDB client and database
            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
                return new MongoClient(settings!.ConnectionString);
            });

            services.AddSingleton<IMongoDatabase>(serviceProvider =>
            {
                var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
                var client = serviceProvider.GetRequiredService<IMongoClient>();
                return client.GetDatabase(settings!.DatabaseName);
            });

            // Register DbContext
            services.AddScoped<AppDbContext>();

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEVOwnerRepository, EVOwnerRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            // Register services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordService, PasswordService>();

            return services;
        }

        private static void ConfigureMongoDbConventions()
        {
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new IgnoreIfDefaultConvention(true),
                new EnumRepresentationConvention(MongoDB.Bson.BsonType.String)
            };

            ConventionRegistry.Register("AppDBSystemConventions", conventionPack, t => true);
        }
    }
}
