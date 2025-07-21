using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Common.Enums;
using DWS.Messages;
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
    /// <param name="priority">The priority level for processing (Low, Normal, High). Defaults to Normal.</param>
    /// <response code="202">Beatmap fetch request queued successfully</response>
    /// <response code="400">Invalid beatmap ID</response>
    [HttpPost("{id:long}/fetch")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FetchAsync(long id, [FromQuery] MessagePriority priority = MessagePriority.Normal)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid beatmap ID");
        }

        var message = new FetchBeatmapMessage
        {
            BeatmapId = id,
            RequestedAt = DateTime.UtcNow,
            Priority = priority
        };

        await publishEndpoint.Publish(message, context =>
        {
            context.SetPriority((byte)message.Priority);
        });

        return Accepted(new { correlationId = message.CorrelationId, beatmapId = id, priority });
    }
}
