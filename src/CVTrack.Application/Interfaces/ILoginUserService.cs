using System.Threading.Tasks;
using CVTrack.Application.Users.Commands;

namespace CVTrack.Application.Interfaces;

public interface ILoginUserService
{    
    Task<string?> LoginAsync(LoginUserCommand command);
}
