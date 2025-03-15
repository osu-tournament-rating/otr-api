using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Common.Enums;
using Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public partial class PlayersController(
    IPlayerService playerService,
    IAdminNoteService adminNoteService,
    IPlayerStatsService playerStatsService
) : Controller
{
    /// <summary>
    /// Get a player
    /// </summary>
    /// <remarks>Get a player searching first by id, then by osu! id, then osu! username</remarks>
    /// <param name="key">Search key (id, osu! id, or osu! username)</param>
    /// <response code="404">A player matching the given key does not exist</response>
    /// <response code="200">Returns a player</response>
    [HttpGet("{key}")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
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
    [HttpGet("{key}/stats")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
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
    /// List all admin notes for a player
    /// </summary>
    /// <param name="id">Player id</param>
    /// <response code="404">A player matching the given id does not exist</response>
    /// <response code="200">Returns all admin notes from a player</response>
    [HttpGet("{id:int}/notes")]
    [Authorize(Roles = $"{OtrClaims.Roles.User}, {OtrClaims.Roles.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<AdminNoteDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAdminNotesAsync(int id)
    {
        if (!await playerService.ExistsAsync(id))
        {
            return NotFound();
        }

        return Ok(await adminNoteService.ListAsync<PlayerAdminNote>(id));
    }
}
