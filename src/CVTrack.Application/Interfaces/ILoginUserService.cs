using System.Threading.Tasks;
using CVTrack.Application.Users.Commands;

namespace CVTrack.Application.Interfaces;

/// <summary>
/// Kullanıcı kimlik doğrulama (login) servisi için sözleşme.
/// </summary>
public interface ILoginUserService
{
    /// <summary>
    /// Geçerli kimlik bilgileriyle başarılı login olursa JWT token döner, aksi halde null.
    /// </summary>
    Task<string?> LoginAsync(LoginUserCommand command);
}
