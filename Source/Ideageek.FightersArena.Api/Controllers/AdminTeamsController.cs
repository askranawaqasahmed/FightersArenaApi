using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/teams")]
public class AdminTeamsController : ApiControllerBase
{
    private readonly ITeamService _teamService;

    public AdminTeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() =>
        ApiOk("Teams retrieved", await _teamService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamRequest request)
    {
        var id = await _teamService.CreateAsync(request);
        return ApiCreated("Team created", new { id });
    }

    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> Update(Guid id, UpdateTeamRequest request)
    {
        await _teamService.UpdateAsync(id, request);
        return ApiOk("Team updated", new { id });
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _teamService.DeleteAsync(id);
        return ApiOk("Team deleted", new { id });
    }
}
