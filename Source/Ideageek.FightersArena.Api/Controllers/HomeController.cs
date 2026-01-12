using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/home")]
public class HomeController : ControllerBase
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _homeService.GetSummaryAsync());
}
