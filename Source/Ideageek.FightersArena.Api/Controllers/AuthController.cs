using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(AuthRegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        if (response is null)
        {
            return ApiError(HttpStatusCode.BadRequest, "Registration failed");
        }

        return ApiOk("Registration succeeded", response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(AuthLoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response is null)
        {
            return ApiError(HttpStatusCode.Unauthorized, "Invalid credentials");
        }

        return ApiOk("Login succeeded", response);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
        var profile = new
        {
            UserId = claims.TryGetValue(ClaimTypes.NameIdentifier, out var id) ? id : string.Empty,
            Email = claims.TryGetValue(ClaimTypes.Email, out var email) ? email : claims.GetValueOrDefault("email"),
            Name = claims.GetValueOrDefault(ClaimTypes.Name)
        };

        return ApiOk("User profile", profile);
    }
}
