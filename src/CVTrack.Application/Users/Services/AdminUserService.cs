using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.Users.Commands;
using CVTrack.Application.Users.Queries;
using CVTrack.Domain.Common;
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

    public async Task<PagedResult<AdminUserDto>> GetAllPagedAsync(GetAllUsersQuery query)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        PagedResult<User> pagedUsers;

        // Search varsa
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            pagedUsers = await _userRepo.SearchUsersPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize,
                query.SearchTerm
            );
        }
        // Role filter varsa
        else if (query.Role.HasValue)
        {
            pagedUsers = await _userRepo.GetUsersByRolePagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize,
                query.Role.Value
            );
        }
        // Normal pagination
        else
        {
            pagedUsers = await _userRepo.GetPagedAsync(
                pagination.ValidatedPageNumber,
                pagination.ValidatedPageSize
            );
        }

        // User'ları AdminUserDto'ya dönüştür
        var adminUserDtos = pagedUsers.Items.Select(u => new AdminUserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Role = u.Role.ToString()
        });

        return new PagedResult<AdminUserDto>
        {
            Items = adminUserDtos,
            TotalCount = pagedUsers.TotalCount,
            PageNumber = pagedUsers.PageNumber,
            PageSize = pagedUsers.PageSize
        };
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