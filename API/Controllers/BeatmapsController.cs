using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Authorize(Roles = "system, admin")] // Internal access only at this time
[Route("api/v{version:apiVersion}/[controller]")]
public class BeatmapsController(IBeatmapService beatmapService) : Controller
{
    private readonly IBeatmapService _beatmapService = beatmapService;

    [HttpGet]
    [EndpointSummary("List beatmaps")]
    public async Task<Ok<IEnumerable<BeatmapDTO>>> ListAsync() =>
        TypedResults.Ok(await _beatmapService.ListAsync());

    [HttpGet("{key}")]
    [EndpointSummary("Get a beatmap by versatile search")]
    [EndpointDescription("Get a beatmap searching first by id, then by osu! beatmap id")]
    public async Task<Results<NotFound, Ok<BeatmapDTO>>> GetAsync(string key)
    {
        BeatmapDTO? beatmap = await _beatmapService.GetVersatileAsync(key);
        if (beatmap == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(beatmap);
    }
}
