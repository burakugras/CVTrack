using CVTrack.Application.Users.Commands;
using CVTrack.Application.Interfaces;
using CVTrack.Application.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace CVTrack.Application.Users.Services;

public class LoginUserService
{
    private readonly IUserRepository _userRepository;

    public LoginUserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> LoginAsync(LoginUserCommand command)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user == null)
            return null;

        // Girilen şifreyle veritabanındaki hash’i karşılaştır
        var hashedInput = HashPassword(command.Password);
        if (user.PasswordHash != hashedInput)
            return null;

        // Başarılıysa DTO döndür
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}