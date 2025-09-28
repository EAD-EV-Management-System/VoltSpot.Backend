using Domain.Entities;
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
    }
}
