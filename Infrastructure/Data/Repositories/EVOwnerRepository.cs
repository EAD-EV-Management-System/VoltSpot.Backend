using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
using MongoDB.Driver;

namespace Infrastructure.Data.Repositories
{
    public class EVOwnerRepository : BaseRepository<EVOwner>, IEVOwnerRepository
    {
        public EVOwnerRepository(AppDbContext context) : base(context.EVOwners)
        {
        }

        public async Task<EVOwner?> GetByNICAsync(string nic)
        {
            return await _collection.Find(x => x.NIC == nic && !x.IsDeleted)
                                  .FirstOrDefaultAsync();
        }

        public async Task<EVOwner?> GetByEmailAsync(string email)
        {
            return await _collection.Find(x => x.Email == email && !x.IsDeleted)
                                  .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByNICAsync(string nic)
        {
            return await _collection.CountDocumentsAsync(x => x.NIC == nic && !x.IsDeleted) > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _collection.CountDocumentsAsync(x => x.Email == email && !x.IsDeleted) > 0;
        }

        // Original method returning IEnumerable for compatibility
        public async Task<IEnumerable<EVOwner>> GetAllAsync()
        {
            return await _collection.Find(x => !x.IsDeleted)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // New method for admin view with pagination and filtering
        public async Task<List<EVOwner>> GetAllAsync(int page = 1, int pageSize = 50, string? status = null, string? searchTerm = null)
        {
            var filter = Builders<EVOwner>.Filter.Eq(x => x.IsDeleted, false);

            // Apply status filter
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<AccountStatus>(status, true, out var accountStatus))
            {
                filter &= Builders<EVOwner>.Filter.Eq(x => x.Status, accountStatus);
            }

            // Apply search term filter (search in name, email, NIC)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchFilter = Builders<EVOwner>.Filter.Or(
                    Builders<EVOwner>.Filter.Regex(x => x.FirstName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<EVOwner>.Filter.Regex(x => x.LastName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<EVOwner>.Filter.Regex(x => x.Email, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<EVOwner>.Filter.Regex(x => x.NIC, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
                );
                filter &= searchFilter;
            }

            return await _collection.Find(filter)
                .SortByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }
    }
}
