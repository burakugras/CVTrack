namespace CVTrack.Application.Users.Commands;

public class UpdateUserRoleCommand
{
    public Guid UserId { get; set; }
    public string NewRole { get; set; } = null!;  // "User" veya "Admin"
}