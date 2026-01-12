using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/tournaments")]
public class AdminTournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public AdminTournamentsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _tournamentService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateTournamentRequest request)
    {
        var id = await _tournamentService.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> Update(Guid id, CreateTournamentRequest request)
    {
        await _tournamentService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _tournamentService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/stages")]
    public async Task<IActionResult> AddStage(Guid id, AddStageRequest request)
    {
        await _tournamentService.AddStageAsync(id, request);
        return NoContent();
    }

    [HttpPost("/api/admin/stages/{stageId:guid}/participants")]
    public async Task<IActionResult> AddParticipants(Guid stageId, AddStageParticipantsRequest request)
    {
        await _tournamentService.AddParticipantsAsync(stageId, request);
        return NoContent();
    }

    [HttpPost("/api/admin/stages/{stageId:guid}/generate-matches")]
    public async Task<IActionResult> GenerateMatches(Guid stageId, GenerateMatchesRequest request)
    {
        var matches = await _tournamentService.GenerateMatchesAsync(stageId, request);
        return Ok(matches);
    }

    [HttpPost("/api/admin/matches/{matchId:guid}/result")]
    public async Task<IActionResult> RecordResult(Guid matchId, RecordMatchResultRequest request)
    {
        await _tournamentService.RecordResultAsync(matchId, request);
        return NoContent();
    }

    [HttpPost("{id:guid}/finalize")]
    public async Task<IActionResult> Finalize(Guid id, FinalizeTournamentRequest request)
    {
        await _tournamentService.FinalizeAsync(id, request);
        return NoContent();
    }
}
