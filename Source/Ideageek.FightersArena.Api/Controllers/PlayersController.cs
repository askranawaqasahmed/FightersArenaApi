using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/players")]
public class PlayersController : ApiControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        ApiOk("Players retrieved", await _playerService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var player = await _playerService.GetAsync(id);
        if (player is null) return ApiError(HttpStatusCode.NotFound, "Player not found");

        return ApiOk("Player retrieved", player);
    }
}
