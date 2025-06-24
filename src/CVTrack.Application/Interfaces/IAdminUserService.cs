using CVTrack.Application.DTOs;
using CVTrack.Application.Users.Commands;
using CVTrack.Application.Users.Queries;
using CVTrack.Domain.Common;

namespace CVTrack.Application.Interfaces;

public interface IAdminUserService
{
    Task<IEnumerable<AdminUserDto>> GetAllAsync(GetAllUsersQuery query);
    Task UpdateRoleAsync(UpdateUserRoleCommand command);
    Task DeleteAsync(DeleteUserCommand command);

    Task<PagedResult<AdminUserDto>> GetAllPagedAsync(GetAllUsersQuery query);
}