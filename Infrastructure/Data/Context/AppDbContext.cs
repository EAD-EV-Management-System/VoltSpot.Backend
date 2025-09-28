using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Data.Context
{
    public class AppDbContext
    {
        private readonly IMongoDatabase _database;
        public AppDbContext(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<EVOwner> EVOwners => _database.GetCollection<EVOwner>("EVOwners");
    }
}
