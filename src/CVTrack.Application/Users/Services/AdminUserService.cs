using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.Users.Commands;
using CVTrack.Application.Users.Queries;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public class AdminUserService : IAdminUserService
{
    private readonly IUserRepository _userRepo;

    public AdminUserService(IUserRepository userRepo)
        => _userRepo = userRepo;

    public async Task<IEnumerable<AdminUserDto>> GetAllAsync(GetAllUsersQuery _)
    {
        var users = await _userRepo.GetAllAsync();
        return users.Select(u => new AdminUserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Role = u.Role.ToString()
        });
    }

    public async Task UpdateRoleAsync(UpdateUserRoleCommand cmd)
    {
        var user = await _userRepo.GetByIdAsync(cmd.UserId)
                   ?? throw new KeyNotFoundException($"User Id={cmd.UserId} bulunamadı.");

        if (!Enum.TryParse<UserRole>(cmd.NewRole, ignoreCase: true, out var role))
            throw new ArgumentException($"Geçersiz role: {cmd.NewRole}");

        user.Role = role;
        await _userRepo.UpdateAsync(user);
    }

    public async Task DeleteAsync(DeleteUserCommand cmd)
    {
        var user = await _userRepo.GetByIdAsync(cmd.UserId)
                   ?? throw new KeyNotFoundException($"User Id={cmd.UserId} bulunamadı.");

        await _userRepo.RemoveAsync(user);
    }
}