using CVTrack.Application.DTOs;
using CVTrack.Application.Users.Commands;
using CVTrack.Application.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly RegisterUserService _registerService;

    public UsersController(RegisterUserService registerService)
    {
        _registerService = registerService;
    }

    // POST: api/users/register
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserCommand cmd)
    {
        var created = await _registerService.RegisterAsync(cmd);
        return CreatedAtAction(null, new { email = created.Email }, created);
    }

    // TODO: login, get-by-email vb. endpoint'ler burada eklenecek
}
