using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class StatsController(
    IPlayerStatsService playerStatsService,
    IBaseStatsService baseStatsService,
    IPlayerService playerService
    ) : Controller
{
    /// <summary>
    /// Get a player's stats
    /// </summary>
    /// <remarks>Not specifying a date range will return all player stats</remarks>
    /// <param name="id">Id of the player</param>
    /// <param name="comparerId">Id of a player to compare stats with the target player</param>
    /// <param name="mode">osu! ruleset. If null, osu! Standard is used</param>
    /// <param name="dateMin">Filter from earliest date. If null, earliest possible date</param>
    /// <param name="dateMax">Filter to latest date. If null, latest possible date</param>
    /// <response code="404">If a player does not exist</response>
    /// <response code="200">Returns a player's stats</response>
    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{OtrClaims.User}, {OtrClaims.Client}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<PlayerStatsDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        int id,
        [FromQuery] int? comparerId,
        [FromQuery] int mode = 0,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null)
    {
        if (!await playerService.ExistsAsync(id))
        {
            return NotFound();
        }

        return Ok(await playerStatsService.GetAsync(
            id,
            comparerId,
            mode,
            dateMin ?? DateTime.MinValue,
            dateMax ?? DateTime.MaxValue
        ));
    }

    [HttpGet("{username}")]
    [Authorize(Roles = $"{OtrClaims.User}, {OtrClaims.Client}")]
    public async Task<ActionResult<PlayerStatsDTO?>> GetByUsernameAsync(
        string username,
        [FromQuery] int? comparerId,
        [FromQuery] int mode = 0,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    ) =>
        await playerStatsService.GetAsync(
            username,
            comparerId,
            mode,
            dateMin ?? DateTime.MinValue,
            dateMax ?? DateTime.UtcNow
        );

    [HttpGet("histogram")]
    [Authorize(Roles = $"{OtrClaims.User}, {OtrClaims.Client}")]
    public async Task<ActionResult<IDictionary<int, int>>> GetRatingHistogramAsync(
        [FromQuery] int mode = 0
    ) => Ok(await baseStatsService.GetHistogramAsync(mode));

    [HttpPost("ratingadjustments")]
    [Authorize(Roles = OtrClaims.System)]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<RatingAdjustmentDTO> postBody)
    {
        await playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpPost("matchstats")]
    [Authorize(Roles = OtrClaims.System)]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<PlayerMatchStatsDTO> postBody)
    {
        await playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpPost("ratingstats")]
    [Authorize(Roles = OtrClaims.System)]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<MatchRatingStatsDTO> postBody)
    {
        await playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpPost("basestats")]
    [Authorize(Roles = OtrClaims.System)]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<BaseStatsPostDTO> postBody)
    {
        await playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpPost("gamewinrecords")]
    [Authorize(Roles = OtrClaims.System)]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<GameWinRecordDTO> postBody)
    {
        await playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpPost("matchwinrecords")]
    [Authorize(Roles = OtrClaims.System)]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<MatchWinRecordDTO> postBody)
    {
        await playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpDelete]
    [Authorize(Roles = OtrClaims.System)]
    public async Task<IActionResult> TruncateAsync()
    {
        await playerStatsService.TruncateAsync();
        return Ok();
    }
}
