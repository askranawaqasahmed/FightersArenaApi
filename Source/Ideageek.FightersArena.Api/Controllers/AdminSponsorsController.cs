using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/sponsors")]
public class AdminSponsorsController : ApiControllerBase
{
    private readonly ISponsorService _sponsorService;

    public AdminSponsorsController(ISponsorService sponsorService)
    {
        _sponsorService = sponsorService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() =>
        ApiOk("Sponsors retrieved", await _sponsorService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateSponsorRequest request)
    {
        var id = await _sponsorService.CreateAsync(request);
        return ApiCreated("Sponsor created", new { id });
    }

    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> Update(Guid id, CreateSponsorRequest request)
    {
        await _sponsorService.UpdateAsync(id, request);
        return ApiOk("Sponsor updated", new { id });
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sponsorService.DeleteAsync(id);
        return ApiOk("Sponsor deleted", new { id });
    }
}
