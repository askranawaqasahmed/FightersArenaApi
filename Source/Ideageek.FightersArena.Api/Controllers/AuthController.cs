using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(AuthRegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        if (response is null) return BadRequest("Registration failed");
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(AuthLoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response is null) return Unauthorized();
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> Me()
    {
        var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
        return Ok(new
        {
            UserId = claims.TryGetValue(ClaimTypes.NameIdentifier, out var id) ? id : string.Empty,
            Email = claims.TryGetValue(ClaimTypes.Email, out var email) ? email : claims.GetValueOrDefault("email"),
            Name = claims.GetValueOrDefault(ClaimTypes.Name)
        });
    }
}
