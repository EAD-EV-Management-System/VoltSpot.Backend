using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context.Users)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _collection.Find(x => x.Username == username && !x.IsDeleted)
                                  .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _collection.Find(x => x.Email == email && !x.IsDeleted)
                                  .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _collection.Find(x => x.Role == role && !x.IsDeleted)
                                  .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync(UserRole? role = null, AccountStatus? status = null, string? searchTerm = null)
        {
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq(u => u.IsDeleted, false);

            if (role.HasValue)
            {
                filter &= filterBuilder.Eq(u => u.Role, role.Value);
            }

            if (status.HasValue)
            {
                filter &= filterBuilder.Eq(u => u.Status, status.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchFilter = filterBuilder.Or(
                    filterBuilder.Regex(u => u.Username, new BsonRegularExpression(searchTerm, "i")),
                    filterBuilder.Regex(u => u.Email, new BsonRegularExpression(searchTerm, "i")),
                    filterBuilder.Regex(u => u.FirstName, new BsonRegularExpression(searchTerm, "i")),
                    filterBuilder.Regex(u => u.LastName, new BsonRegularExpression(searchTerm, "i"))
                );
                filter &= searchFilter;
            }

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _collection.CountDocumentsAsync(x => x.Username == username && !x.IsDeleted) > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _collection.CountDocumentsAsync(x => x.Email == email && !x.IsDeleted) > 0;
        }
    }
}
