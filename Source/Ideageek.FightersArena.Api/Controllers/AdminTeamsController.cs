using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/teams")]
public class AdminTeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public AdminTeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _teamService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamRequest request)
    {
        var id = await _teamService.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> Update(Guid id, UpdateTeamRequest request)
    {
        await _teamService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _teamService.DeleteAsync(id);
        return NoContent();
    }
}
