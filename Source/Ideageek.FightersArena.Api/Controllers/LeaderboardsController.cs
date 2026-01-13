using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/leaderboards")]
public class LeaderboardsController : ApiControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardsController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent([FromQuery] string type = "Player", [FromQuery] int top = 10, [FromQuery] Guid? gameId = null)
    {
        var leaderboard = await _leaderboardService.GetCurrentAsync(type, top, gameId);
        return ApiOk("Leaderboard retrieved", leaderboard);
    }
}
