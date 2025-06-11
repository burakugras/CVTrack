using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
}