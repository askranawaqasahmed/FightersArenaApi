using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/teams")]
public class TeamsController : ApiControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        ApiOk("Teams retrieved", await _teamService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var team = await _teamService.GetAsync(id);
        if (team is null) return ApiError(HttpStatusCode.NotFound, "Team not found");

        return ApiOk("Team retrieved", team);
    }
}
