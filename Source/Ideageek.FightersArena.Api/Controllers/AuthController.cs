using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Entities.Authorization;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserStore<AspNetUser> _userStore;

    public AuthController(IAuthService authService, IUserStore<AspNetUser> userStore)
    {
        _authService = authService;
        _userStore = userStore;
    }

    [HttpPost("signupmobile")]
    [AllowAnonymous]
    public async Task<IActionResult> SignupMobile(AuthRegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request, "Player");
            if (response is null)
            {
                return ApiError(HttpStatusCode.BadRequest, "Signup failed");
            }

            return ApiOk(response.Token);
        }
        catch (InvalidOperationException ex)
        {
            return ApiError(HttpStatusCode.BadRequest, ex.Message);
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(AuthRegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            if (response is null)
            {
                return ApiError(HttpStatusCode.BadRequest, "Registration failed");
            }

            return ApiOk(response.Token);
        }
        catch (InvalidOperationException ex)
        {
            return ApiError(HttpStatusCode.BadRequest, ex.Message);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(AuthLoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            if (response is null)
            {
                return ApiError(HttpStatusCode.Unauthorized, "Invalid credentials");
            }

            return ApiOk(response.Token);
        }
        catch (InvalidOperationException ex)
        {
            return ApiError(HttpStatusCode.BadRequest, ex.Message);
        }
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await _authService.SendResetLinkAsync(request.Email);
        return ApiAccepted("If an account exists for this email, reset instructions have been sent.");
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
            Name = claims.GetValueOrDefault(ClaimTypes.Name),
            Phone = claims.GetValueOrDefault(ClaimTypes.MobilePhone)
        };

        return ApiOk("User profile", profile);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var id))
        {
            return ApiError(HttpStatusCode.Unauthorized, "User not found in token");
        }

        var user = await _userStore.FindByIdAsync(id.ToString(), CancellationToken.None);
        if (user is null)
        {
            return ApiError(HttpStatusCode.NotFound, "User not found");
        }

        var profile = new
        {
            user.Id,
            user.FullName,
            user.Email,
            user.UserName,
            user.PhoneNumber
        };

        return ApiOk("User profile", profile);
    }
}
