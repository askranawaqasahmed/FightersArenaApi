using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/leagues")]
public class LeaguesController : ApiControllerBase
{
    private readonly ILeagueService _leagueService;

    public LeaguesController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        ApiOk("Leagues retrieved", await _leagueService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var league = await _leagueService.GetAsync(id);
        if (league is null) return ApiError(HttpStatusCode.NotFound, "League not found");
        return ApiOk("League retrieved", league);
    }

    [HttpGet("{id:guid}/fixtures")]
    public async Task<IActionResult> GetFixtures(Guid id) =>
        ApiOk("League fixtures", await _leagueService.GetFixturesAsync(id));

    [HttpGet("{id:guid}/standings")]
    public async Task<IActionResult> GetStandings(Guid id) =>
        ApiOk("League standings", await _leagueService.GetStandingsAsync(id));
}
