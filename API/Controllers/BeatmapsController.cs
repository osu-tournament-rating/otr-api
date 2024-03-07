using API.Entities;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Authorize(Roles = "system")] // Internal access only at this time
[Route("api/v{version:apiVersion}/[controller]")]
public class BeatmapsController(IBeatmapService beatmapService) : Controller
{
    private readonly IBeatmapService _beatmapService = beatmapService;

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Beatmap>>> GetAllAsync() =>
        Ok(await _beatmapService.GetAllAsync());

    [HttpGet("{beatmapId:long}")]
    public async Task<ActionResult<Beatmap>> GetByOsuBeatmapIdAsync(long beatmapId)
    {
        DTOs.BeatmapDTO? beatmap = await _beatmapService.GetAsync(beatmapId);
        if (beatmap == null)
        {
            return NotFound("No matching beatmapId in the database.");
        }

        return Ok(beatmap);
    }
}
