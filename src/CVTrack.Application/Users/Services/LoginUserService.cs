using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CVTrack.Application.Interfaces;
using CVTrack.Application.Users.Commands;
using CVTrack.Domain.Entities;

namespace CVTrack.Application.Users.Services;

public class LoginUserService : ILoginUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginUserService(
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    // Başarılı login olursa JWT token döner; başarısızsa null.
    public async Task<string?> LoginAsync(LoginUserCommand command)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user == null)
            return null;

        // Şifre karşılaştırması
        var hashedInput = HashPassword(command.Password);
        if (user.PasswordHash != hashedInput)
            return null;

        // Token üretirken role bilgisini de veriyoruz
        var token = _tokenService.CreateToken(
            user.Id,
            user.Email,
            user.Role,
            user.FirstName,
            user.LastName);

        return token;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

