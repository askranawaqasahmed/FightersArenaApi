using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/tournaments")]
public class AdminTournamentsController : ApiControllerBase
{
    private readonly ITournamentService _tournamentService;

    public AdminTournamentsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() =>
        ApiOk("Tournaments retrieved", await _tournamentService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateTournamentRequest request)
    {
        var id = await _tournamentService.CreateAsync(request);
        return ApiCreated("Tournament created", new { id });
    }

    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> Update(Guid id, CreateTournamentRequest request)
    {
        await _tournamentService.UpdateAsync(id, request);
        return ApiOk("Tournament updated", new { id });
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _tournamentService.DeleteAsync(id);
        return ApiOk("Tournament deleted", new { id });
    }

    [HttpPost("{id:guid}/stages")]
    public async Task<IActionResult> AddStage(Guid id, AddStageRequest request)
    {
        await _tournamentService.AddStageAsync(id, request);
        return ApiOk("Stage added", new { tournamentId = id });
    }

    [HttpPost("/api/admin/stages/{stageId:guid}/participants")]
    public async Task<IActionResult> AddParticipants(Guid stageId, AddStageParticipantsRequest request)
    {
        await _tournamentService.AddParticipantsAsync(stageId, request);
        return ApiOk("Stage participants updated", new { stageId });
    }

    [HttpPost("/api/admin/stages/{stageId:guid}/generate-matches")]
    public async Task<IActionResult> GenerateMatches(Guid stageId, GenerateMatchesRequest request)
    {
        var matches = await _tournamentService.GenerateMatchesAsync(stageId, request);
        return ApiOk("Matches generated", matches);
    }

    [HttpPost("/api/admin/matches/{matchId:guid}/result")]
    public async Task<IActionResult> RecordResult(Guid matchId, RecordMatchResultRequest request)
    {
        await _tournamentService.RecordResultAsync(matchId, request);
        return ApiOk("Match result recorded", new { matchId });
    }

    [HttpPost("{id:guid}/finalize")]
    public async Task<IActionResult> Finalize(Guid id, FinalizeTournamentRequest request)
    {
        await _tournamentService.FinalizeAsync(id, request);
        return ApiOk("Tournament finalized", new { id });
    }
}
