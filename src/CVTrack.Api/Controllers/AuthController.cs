using CVTrack.Application.Interfaces;
using CVTrack.Application.Users.Commands;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILoginUserService _loginService;

    public AuthController(ILoginUserService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginUserCommand cmd)
    {
        var token = await _loginService.LoginAsync(cmd);
        if (string.IsNullOrEmpty(token))
            return Unauthorized("Geçersiz kimlik bilgisi.");

        return Ok(token);
    }
}

