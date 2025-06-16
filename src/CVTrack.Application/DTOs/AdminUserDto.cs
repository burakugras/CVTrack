using CVTrack.Domain.Entities;

namespace CVTrack.Application.DTOs;

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!; // "User" veya "Admin"

}