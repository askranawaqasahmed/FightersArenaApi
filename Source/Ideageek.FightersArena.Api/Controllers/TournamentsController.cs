using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/tournaments")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _tournamentService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var tournament = await _tournamentService.GetAsync(id);
        return tournament is null ? NotFound() : Ok(tournament);
    }
}
