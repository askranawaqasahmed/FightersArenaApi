using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/games")]
public class AdminGamesController : ApiControllerBase
{
    private readonly IGameService _gameService;

    public AdminGamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() =>
        ApiOk("Games retrieved", await _gameService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateGameRequest request)
    {
        var id = await _gameService.CreateAsync(request);
        return ApiCreated("Game created", new { id });
    }

    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> Update(Guid id, CreateGameRequest request)
    {
        await _gameService.UpdateAsync(id, request);
        return ApiOk("Game updated", new { id });
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _gameService.DeleteAsync(id);
        return ApiOk("Game deleted", new { id });
    }
}
