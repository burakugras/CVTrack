using CVTrack.Domain.Entities;

namespace CVTrack.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(Guid userId, string email, UserRole userRole, string firtName, string lastName);
}
