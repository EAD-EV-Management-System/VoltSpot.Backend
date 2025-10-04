// Infrastructure/Data/MongoDbContext.cs
/*
 * File: MongoDbContext.cs
 * Purpose: MongoDB database configuration and connection
 * Author: [Your Name]
 * Date: [Current Date]
 */

using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace VoltSpot.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            _database = client.GetDatabase("VoltSpotDb");
        }

        public IMongoDatabase Database => _database;
    }
}