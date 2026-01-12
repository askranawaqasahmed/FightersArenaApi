using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/seasons")]
public class AdminSeasonsController : ControllerBase
{
    private readonly ISeasonService _seasonService;

    public AdminSeasonsController(ISeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _seasonService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateSeasonRequest request)
    {
        var id = await _seasonService.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> Update(Guid id, CreateSeasonRequest request)
    {
        await _seasonService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _seasonService.DeleteAsync(id);
        return NoContent();
    }
}
