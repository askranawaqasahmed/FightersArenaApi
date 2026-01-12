using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/teams")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _teamService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var team = await _teamService.GetAsync(id);
        return team is null ? NotFound() : Ok(team);
    }
}
