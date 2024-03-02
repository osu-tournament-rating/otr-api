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
[Authorize(Roles = "user")]
[Route("api/v{version:apiVersion}/[controller]")]
public class StatsController : Controller
{
	private readonly IBaseStatsService _baseStatsService;
	private readonly IPlayerStatsService _playerStatsService;

	public StatsController(IPlayerStatsService playerStatsService, IBaseStatsService baseStatsService)
	{
		_playerStatsService = playerStatsService;
		_baseStatsService = baseStatsService;
	}

	[HttpGet("{playerId:int}")]
	public async Task<ActionResult<PlayerStatsDTO>> GetAsync(int playerId, [FromQuery] int? comparerId, [FromQuery] int mode = 0, [FromQuery] DateTime? dateMin = null,
		[FromQuery]
		DateTime? dateMax = null) => await _playerStatsService.GetAsync(playerId, comparerId, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.UtcNow);

	[HttpGet("{username}")]
	public async Task<ActionResult<PlayerStatsDTO?>> GetAsync(string username, [FromQuery] int? comparerId, [FromQuery] int mode = 0, [FromQuery] DateTime? dateMin = null,
		[FromQuery]
		DateTime? dateMax = null) => await _playerStatsService.GetAsync(username, comparerId, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.UtcNow);

	[HttpGet("histogram")]
	public async Task<ActionResult<IDictionary<int, int>>> GetRatingHistogramAsync([FromQuery] int mode = 0) => Ok(await _baseStatsService.GetHistogramAsync(mode));

	[HttpPost("ratingadjustments")]
	[Authorize(Roles = "system")]
	public async Task<IActionResult> PostAsync([FromBody] IEnumerable<RatingAdjustmentDTO> postBody)
	{
		await _playerStatsService.BatchInsertAsync(postBody);
		return Ok();
	}

	[HttpDelete("ratingadjustments")]
	[Authorize(Roles = "system")]
	public async Task<IActionResult> TruncateAdjustmentsAsync()
	{
		await _playerStatsService.TruncateRatingAdjustmentsAsync();
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