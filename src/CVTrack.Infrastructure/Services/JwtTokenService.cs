using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CVTrack.Infrastructure.Services;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _config;
    public JwtTokenService(IConfiguration config) => _config = config;

    public string CreateToken(Guid userId, string email, UserRole userRole, string firstName, string lastName)
    {
        var jwtSection = _config.GetSection("JwtSettings");
        var keyBytes = Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!);
        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,   userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userRole.ToString()),
                new Claim("firstName", firstName),
                new Claim("lastName", lastName)
            };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpiresInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}