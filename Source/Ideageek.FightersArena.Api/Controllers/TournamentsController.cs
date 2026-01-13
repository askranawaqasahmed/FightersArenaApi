using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/tournaments")]
public class TournamentsController : ApiControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        ApiOk("Tournaments retrieved", await _tournamentService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var tournament = await _tournamentService.GetAsync(id);
        if (tournament is null) return ApiError(HttpStatusCode.NotFound, "Tournament not found");

        return ApiOk("Tournament retrieved", tournament);
    }
}
