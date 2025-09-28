using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
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
