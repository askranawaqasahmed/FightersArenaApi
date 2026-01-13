using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Route("api/home")]
public class HomeController : ApiControllerBase
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() =>
        ApiOk("Home summary retrieved", await _homeService.GetSummaryAsync());
}
