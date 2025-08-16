using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.DataAnnotations;
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
public class BeatmapsController(IBeatmapService beatmapService, IPublishEndpoint publishEndpoint, ILogger<BeatmapsController> logger) : Controller
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
    /// Enqueues a message to fetch beatmap data from the osu! API
    /// </summary>
    /// <param name="id">The osu! beatmap ID to fetch data for</param>
    /// <param name="priority">The message queue priority for processing this fetch request</param>
    /// <response code="202">The fetch request was accepted and queued for processing</response>
    /// <response code="400">The provided beatmap ID is negative</response>
    [HttpPost("{id:long}/fetch")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType<QueueResponseDTO>(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FetchAsync([Positive] long id, [FromQuery] MessagePriority priority = MessagePriority.Normal)
    {
        var message = new FetchBeatmapMessage
        {
            BeatmapId = id,
            RequestedAt = DateTime.UtcNow,
            Priority = priority
        };

        await publishEndpoint.Publish(message, context =>
        {
            context.SetPriority((byte)message.Priority);
            context.CorrelationId = message.CorrelationId;
        });

        logger.LogInformation("Published beatmap fetch message [Beatmap ID: {BeatmapId} | Correlation ID: {CorrelationId} | Priority: {Priority}]",
            id, message.CorrelationId, priority);

        var response = new QueueResponseDTO
        {
            CorrelationId = message.CorrelationId,
            Priority = priority
        };

        return Accepted(response);
    }
}
