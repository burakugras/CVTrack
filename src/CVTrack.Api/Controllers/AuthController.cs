using CVTrack.Application.Interfaces;
using CVTrack.Application.Users.Services;
using CVTrack.Application.Users.Commands;
using Microsoft.AspNetCore.Mvc;

namespace CVTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUserService _loginService;
    private readonly ITokenService _tokenService;

    public AuthController(LoginUserService loginService, ITokenService tokenService)
    {
        _loginService = loginService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginUserCommand cmd)
    {
        var user = await _loginService.LoginAsync(cmd);
        if (user is null) return Unauthorized("Geçersiz kimlik bilgisi.");

        var token = _tokenService.CreateToken(user.Id, user.Email);
        return Ok(token);
    }
}