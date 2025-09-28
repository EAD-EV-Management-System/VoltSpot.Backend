using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IEVOwnerRepository
    {
        Task<EVOwner?> GetByIdAsync(string id);
        Task<EVOwner?> GetByNICAsync(string nic);
        Task<EVOwner?> GetByEmailAsync(string email);
        Task<IEnumerable<EVOwner>> GetAllAsync();
        Task<EVOwner> AddAsync(EVOwner evOwner);
        Task<EVOwner> UpdateAsync(EVOwner evOwner);
        Task DeleteAsync(string id);
        Task<bool> ExistsByNICAsync(string nic);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
