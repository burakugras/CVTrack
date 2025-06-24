using CVTrack.Domain.Common;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task UpdateAsync(User user);
    Task RemoveAsync(User user);

    Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize);
    Task<PagedResult<User>> GetUsersByRolePagedAsync(int pageNumber, int pageSize, UserRole role);
    Task<PagedResult<User>> SearchUsersPagedAsync(int pageNumber, int pageSize, string searchTerm);
}
