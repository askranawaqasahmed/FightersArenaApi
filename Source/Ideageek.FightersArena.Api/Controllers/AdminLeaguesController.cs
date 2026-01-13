using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin,SuperAdmin,Organizer")]
[Route("api/admin/leagues")]
public class AdminLeaguesController : ApiControllerBase
{
    private readonly ILeagueService _leagueService;

    public AdminLeaguesController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() =>
        ApiOk("Leagues retrieved", await _leagueService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateLeagueRequest request)
    {
        var id = await _leagueService.CreateAsync(request);
        return ApiCreated("League created", new { id });
    }

    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> Update(Guid id, UpdateLeagueRequest request)
    {
        await _leagueService.UpdateAsync(id, request);
        return ApiOk("League updated", new { id });
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _leagueService.DeleteAsync(id);
        return ApiOk("League deleted", new { id });
    }

    [HttpPost("{id:guid}/participants")]
    public async Task<IActionResult> AddParticipants(Guid id, AddLeagueParticipantsRequest request)
    {
        await _leagueService.AddParticipantsAsync(id, request);
        return ApiOk("League participants updated", new { id });
    }

    [HttpPost("{id:guid}/generate-matches")]
    public async Task<IActionResult> GenerateMatches(Guid id, GenerateLeagueMatchesRequest request)
    {
        var matches = await _leagueService.GenerateMatchesAsync(id, request);
        return ApiOk("League matches generated", matches);
    }

    [HttpPost("matches/{matchId:guid}/result")]
    public async Task<IActionResult> RecordResult(Guid matchId, RecordLeagueMatchResultRequest request)
    {
        await _leagueService.RecordResultAsync(matchId, request);
        return ApiOk("League match result recorded", new { matchId });
    }
}
