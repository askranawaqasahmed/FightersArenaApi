using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/players")]
public class AdminPlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public AdminPlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _playerService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreatePlayerRequest request)
    {
        var id = await _playerService.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> Update(Guid id, UpdatePlayerRequest request)
    {
        await _playerService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _playerService.DeleteAsync(id);
        return NoContent();
    }
}
