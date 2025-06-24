using CVTrack.Domain.Entities;

namespace CVTrack.Application.Users.Queries;

public class GetAllUsersQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public UserRole? Role { get; set; }

}
