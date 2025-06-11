namespace CVTrack.Application.Users.Commands;

public class LoginUserCommand
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}