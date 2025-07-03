using API.Authorization;
using API.DTOs;
using API.Messages;
using API.Services.Interfaces;
using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Authorize(Roles = OtrClaims.Roles.Admin)]
[Route("api/v{version:apiVersion}/[controller]")]
public class BeatmapsController(IBeatmapService beatmapService, IPublishEndpoint publishEndpoint) : Controller
{
    /// <summary>
    /// List all beatmaps
    /// </summary>
    /// <response code="200">Returns all beatmaps</response>
    [HttpGet]
    [ProducesResponseType<IEnumerable<BeatmapDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync() =>
        Ok(await beatmapService.ListAsync());

    /// <summary>
    /// Get a beatmap
    /// </summary>
    /// <remarks>Get a beatmap searching first by id, then by osu! id</remarks>
    /// <param name="key">Search key (id or osu! id)</param>
    /// <response code="404">A beatmap matching the given key does not exist</response>
    /// <response code="200">Returns a beatmap</response>
    [HttpGet("{key:long}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BeatmapDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(long key)
    {
        BeatmapDTO? beatmap = await beatmapService.GetVersatileAsync(key);
        if (beatmap == null)
        {
            return NotFound();
        }

        return Ok(beatmap);
    }

    /// <summary>
    /// Queue a beatmap fetch request
    /// </summary>
    /// <remarks>Queues a request to fetch the latest beatmap data from the osu! API</remarks>
    /// <param name="id">The osu! beatmap ID</param>
    /// <response code="202">Beatmap fetch request queued successfully</response>
    /// <response code="400">Invalid beatmap ID</response>
    [HttpPost("{id:long}/fetch")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> QueueFetchAsync(long id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid beatmap ID");
        }

        var message = new FetchBeatmapMessage
        {
            BeatmapId = id,
            RequestedAt = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid()
        };

        await publishEndpoint.Publish(message);

        return Accepted(new { correlationId = message.CorrelationId, beatmapId = id });
    }
}
