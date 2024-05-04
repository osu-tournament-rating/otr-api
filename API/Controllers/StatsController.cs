using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Route("api/v{version:apiVersion}/[controller]")]
public class StatsController(IPlayerStatsService playerStatsService, IBaseStatsService baseStatsService) : Controller
{
    private readonly IBaseStatsService _baseStatsService = baseStatsService;
    private readonly IPlayerStatsService _playerStatsService = playerStatsService;

    [HttpGet("{playerId:int}")]
    [Authorize(Roles = "user, client")]
    public async Task<ActionResult<PlayerStatsDTO>> GetAsync(
        int playerId,
        [FromQuery] int? comparerId,
        [FromQuery] int mode = 0,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    ) =>
        await _playerStatsService.GetAsync(
            playerId,
            comparerId,
            mode,
            dateMin ?? DateTime.MinValue,
            dateMax ?? DateTime.UtcNow
        );

    [HttpGet("{username}")]
    [Authorize(Roles = "user, client")]
    public async Task<ActionResult<PlayerStatsDTO?>> GetAsync(
        string username,
        [FromQuery] int? comparerId,
        [FromQuery] int mode = 0,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    ) =>
        await _playerStatsService.GetAsync(
            username,
            comparerId,
            mode,
            dateMin ?? DateTime.MinValue,
            dateMax ?? DateTime.UtcNow
        );

    [HttpGet("histogram")]
    [Authorize(Roles = "user, client")]
    public async Task<ActionResult<IDictionary<int, int>>> GetRatingHistogramAsync(
        [FromQuery] int mode = 0
    ) => Ok(await _baseStatsService.GetHistogramAsync(mode));

    [HttpPost("ratingadjustments")]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<RatingAdjustmentDTO> postBody)
    {
        await _playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpPost("matchstats")]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<PlayerMatchStatsDTO> postBody)
    {
        await _playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpPost("ratingstats")]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<MatchRatingStatsDTO> postBody)
    {
        await _playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpPost("basestats")]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<BaseStatsPostDTO> postBody)
    {
        await _playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpPost("gamewinrecords")]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<GameWinRecordDTO> postBody)
    {
        await _playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpPost("matchwinrecords")]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> PostAsync([FromBody] IEnumerable<MatchWinRecordDTO> postBody)
    {
        await _playerStatsService.BatchInsertAsync(postBody);
        return Ok();
    }

    [HttpDelete]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> TruncateAsync()
    {
        await _playerStatsService.TruncateAsync();
        return Ok();
    }
}
