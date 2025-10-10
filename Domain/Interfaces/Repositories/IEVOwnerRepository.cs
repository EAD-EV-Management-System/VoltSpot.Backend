using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IEVOwnerRepository
    {
        Task<EVOwner?> GetByIdAsync(string id);
        Task<EVOwner?> GetByNICAsync(string nic);
        Task<EVOwner?> GetByEmailAsync(string email);
        Task<IEnumerable<EVOwner>> GetAllAsync();

        // New method for admin view with pagination and filtering
        Task<List<EVOwner>> GetAllAsync(int page = 1, int pageSize = 50, string? status = null, string? searchTerm = null);

        Task<EVOwner> AddAsync(EVOwner evOwner);
        Task<EVOwner> UpdateAsync(EVOwner evOwner);
        Task DeleteAsync(string id);
        Task<bool> ExistsByNICAsync(string nic);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
