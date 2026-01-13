using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/profile")]
public class ProfileController : ApiControllerBase
{
    private readonly IPlayerService _playerService;

    public ProfileController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = GetUserId();
        if (userId is null) return ApiError(HttpStatusCode.Unauthorized, "User not found in token");

        var profile = await _playerService.GetProfileAsync(userId.Value);
        if (profile is null) return ApiError(HttpStatusCode.NotFound, "Profile not found");

        return ApiOk("Profile retrieved", profile);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdatePlayerRequest request)
    {
        var userId = GetUserId();
        if (userId is null) return ApiError(HttpStatusCode.Unauthorized, "User not found in token");

        var playerId = await _playerService.UpdateProfileAsync(userId.Value, request);
        if (playerId == Guid.Empty) return ApiError(HttpStatusCode.NotFound, "Profile not found");

        return ApiOk("Profile updated", new { id = playerId });
    }

    private Guid? GetUserId()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
        return Guid.TryParse(idValue, out var id) ? id : null;
    }
}
