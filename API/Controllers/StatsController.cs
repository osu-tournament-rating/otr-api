using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class StatsController(
    IPlayerStatsService playerStatsService,
    IBaseStatsService baseStatsService
    ) : Controller
{
    /// <summary>
    /// Get a player's stats
    /// </summary>
    /// <remarks>
    /// Gets player by versatile search.
    /// If no ruleset is provided, the player's default is used. <see cref="Ruleset.Osu"/> is used as a fallback.
    /// If a ruleset is provided but the player has no data for it, all optional fields of the response will be null.
    /// <see cref="PlayerStatsDTO.PlayerInfo"/> will always be populated as long as a player is found.
    /// If no date range is provided, gets all stats without considering date
    /// </remarks>
    /// <param name="key">Key used in versatile search</param>
    /// <param name="ruleset">Ruleset to filter for</param>
    /// <param name="dateMin">Filter from earliest date</param>
    /// <param name="dateMax">Filter to latest date</param>
    /// <response code="404">If a player does not exist</response>
    /// <response code="200">Returns a player's stats</response>
    [HttpGet("{key}")]
    [Authorize(Roles = $"{OtrClaims.User}, {OtrClaims.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<PlayerStatsDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        string key,
        [FromQuery] Ruleset? ruleset = null,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    )
    {
        PlayerStatsDTO? result = await playerStatsService.GetAsync(
            key,
            ruleset,
            dateMin ?? DateTime.MinValue,
            dateMax ?? DateTime.MaxValue
        );

        return result is null
            ? NotFound()
            : Ok(result);
    }

    [HttpGet("histogram")]
    [Authorize(Roles = $"{OtrClaims.User}, {OtrClaims.Client}")]
    public async Task<ActionResult<IDictionary<int, int>>> GetRatingHistogramAsync(
        [FromQuery] Ruleset ruleset = Ruleset.Osu
    ) => Ok(await baseStatsService.GetHistogramAsync(ruleset));
}
