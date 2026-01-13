using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/points")]
public class PointsController : ApiControllerBase
{
    private readonly IPointsService _pointsService;
    private readonly IPlayerService _playerService;

    public PointsController(IPointsService pointsService, IPlayerService playerService)
    {
        _pointsService = pointsService;
        _playerService = playerService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMine([FromQuery] Guid? seasonId = null)
    {
        var userId = GetUserId();
        if (userId is null) return ApiError(HttpStatusCode.Unauthorized, "User not found in token");

        var player = await _playerService.GetByUserIdAsync(userId.Value);
        if (player is null) return ApiError(HttpStatusCode.NotFound, "Player not found");

        var ledger = await _pointsService.GetForParticipantAsync(player.Id, "Player", seasonId);
        return ApiOk("Points history", ledger);
    }

    private Guid? GetUserId()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
        return Guid.TryParse(idValue, out var id) ? id : null;
    }
}
