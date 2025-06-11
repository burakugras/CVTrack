namespace CVTrack.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(Guid userId, string email);
}