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
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(x => x.Username, username),
                filterBuilder.Or(
                    filterBuilder.Eq(x => x.IsDeleted, false),
                    filterBuilder.Exists(x => x.IsDeleted, false)
                )
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(x => x.Email, email),
                filterBuilder.Or(
                    filterBuilder.Eq(x => x.IsDeleted, false),
                    filterBuilder.Exists(x => x.IsDeleted, false)
                )
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(x => x.Role, role),
                filterBuilder.Or(
                    filterBuilder.Eq(x => x.IsDeleted, false),
                    filterBuilder.Exists(x => x.IsDeleted, false)
                )
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync(UserRole? role = null, AccountStatus? status = null, string? searchTerm = null)
        {
            var filterBuilder = Builders<User>.Filter;
            // Handle both cases: documents where isDeleted field exists OR doesn't exist (for backwards compatibility)
            var filter = filterBuilder.Or(
                filterBuilder.Eq(u => u.IsDeleted, false),
                filterBuilder.Exists(u => u.IsDeleted, false) // Field doesn't exist (old documents)
            );

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
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(x => x.Username, username),
                filterBuilder.Or(
                    filterBuilder.Eq(x => x.IsDeleted, false),
                    filterBuilder.Exists(x => x.IsDeleted, false)
                )
            );
            return await _collection.CountDocumentsAsync(filter) > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(x => x.Email, email),
                filterBuilder.Or(
                    filterBuilder.Eq(x => x.IsDeleted, false),
                    filterBuilder.Exists(x => x.IsDeleted, false)
                )
            );
            return await _collection.CountDocumentsAsync(filter) > 0;
        }
    }
}
