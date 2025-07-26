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
[Route("api/v{version:apiVersion}/[controller]/{key}")]
public class PlayersController(IPlayerService playerService, IPlayerStatsService playerStatsService, IPublishEndpoint publishEndpoint, ILogger<PlayersController> logger) : Controller
{
    /// <summary>
    /// Get a player
    /// </summary>
    /// <remarks>Get a player searching first by id, then by osu! id, then osu! username</remarks>
    /// <param name="key">Search key (id, osu! id, or osu! username)</param>
    /// <response code="404">A player matching the given key does not exist</response>
    /// <response code="200">Returns a player</response>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.ApiKeyAuthorization)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<PlayerCompactDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(string key)
    {
        PlayerCompactDTO? result = await playerService.GetVersatileAsync(key);

        return result is not null
            ? Ok(result)
            : NotFound();
    }

    /// <summary>
    /// Get a player's stats
    /// </summary>
    /// <remarks>
    /// Gets player by versatile search.
    /// If no ruleset is provided, the player's default is used. <see cref="Ruleset.Osu"/> is used as a fallback.
    /// If a ruleset is provided but the player has no data for it, all optional fields of the response will be null.
    /// <see cref="PlayerDashboardStatsDTO.PlayerInfo"/> will always be populated as long as a player is found.
    /// If no date range is provided, gets all stats without considering date
    /// </remarks>
    /// <param name="key">Search key</param>
    /// <param name="ruleset">Ruleset to filter for</param>
    /// <param name="dateMin">Filter from earliest date</param>
    /// <param name="dateMax">Filter to latest date</param>
    /// <response code="404">A player matching the given search key does not exist</response>
    /// <response code="200">Returns a player's stats</response>
    [HttpGet("stats")]
    [Authorize(Policy = AuthorizationPolicies.ApiKeyAuthorization)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<PlayerDashboardStatsDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatsAsync(
        string key,
        [FromQuery] Ruleset? ruleset = null,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    )
    {
        PlayerDashboardStatsDTO? result = await playerStatsService.GetAsync(
            key,
            ruleset,
            dateMin ?? DateTime.MinValue,
            dateMax ?? DateTime.MaxValue
        );

        return result is null
            ? NotFound()
            : Ok(result);
    }

    /// <summary>
    /// Get all tournaments a player has participated in
    /// </summary>
    /// <remarks>
    /// Gets tournaments for a player by versatile search.
    /// If no ruleset is provided, returns tournaments from all rulesets.
    /// If no date range is provided, gets all tournaments without date filtering.
    /// </remarks>
    /// <param name="key">Search key (id, osu! id, or osu! username)</param>
    /// <param name="ruleset">Ruleset to filter for</param>
    /// <param name="dateMin">Filter from earliest date</param>
    /// <param name="dateMax">Filter to latest date</param>
    /// <response code="404">A player matching the given key does not exist</response>
    /// <response code="200">Returns a collection of tournaments</response>
    [HttpGet("tournaments")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<TournamentCompactDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTournamentsAsync(
        string key,
        [FromQuery] Ruleset? ruleset = null,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    )
    {
        IEnumerable<TournamentCompactDTO>? result =
            await playerService.GetTournamentsAsync(key, ruleset, dateMin, dateMax);

        return result is not null
            ? Ok(result)
            : NotFound();
    }

    [HttpPost("fetch")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    public async Task<IActionResult> FetchAsync(long key, [FromQuery] MessagePriority priority = MessagePriority.Normal)
    {
        if (key <= 0)
        {
            return BadRequest("Invalid osu! player ID");
        }

        var message = new FetchPlayerMessage
        {
            OsuPlayerId = key,
            RequestedAt = DateTime.UtcNow,
            Priority = priority
        };

        await publishEndpoint.Publish(message, context =>
        {
            context.SetPriority((byte)message.Priority);
            context.CorrelationId = message.CorrelationId;
        });

        logger.LogInformation("Published player fetch message [osu! Player ID: {OsuPlayerId} | Correlation ID: {CorrelationId} | Priority: {Priority}]",
            key, message.CorrelationId, priority);

        return Accepted(new { correlationId = message.CorrelationId, osuPlayerId = key, priority });
    }
}
