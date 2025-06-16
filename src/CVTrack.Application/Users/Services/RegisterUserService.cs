using CVTrack.Application.DTOs;
using CVTrack.Application.Interfaces;
using CVTrack.Application.Users.Commands;
using CVTrack.Domain.Entities;
using System.Security.Cryptography;
using System.Text;


namespace CVTrack.Application.Users.Services;

public class RegisterUserService
{
    private readonly IUserRepository _userRepository;
    public RegisterUserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> RegisterAsync(RegisterUserCommand command)
    {
        if (await _userRepository.ExistsByEmailAsync(command.Email))
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var hashedPassword = HashPassword(command.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PasswordHash = hashedPassword,
            Role        = UserRole.User
        };

        var createdUser = await _userRepository.AddAsync(user);

        return new UserDto
        {
            Id = createdUser.Id,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            Email = createdUser.Email
        };
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

}