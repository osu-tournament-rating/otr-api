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
[Authorize(Roles = "Admin, System")]
public class StatsController : Controller
{
	private readonly IBaseStatsService _baseStatsService;
	private readonly IPlayerStatsService _playerStatsService;

	public StatsController(IPlayerStatsService playerStatsService, IBaseStatsService baseStatsService)
	{
		_playerStatsService = playerStatsService;
		_baseStatsService = baseStatsService;
	}

	[AllowAnonymous]
	[HttpGet("{playerId:int}")]
	public async Task<ActionResult<PlayerStatsDTO>> GetAsync(int playerId, [FromQuery] int? comparerId, [FromQuery] int mode = 0, [FromQuery] DateTime? dateMin = null,
		[FromQuery]
		DateTime? dateMax = null) => await _playerStatsService.GetAsync(playerId, comparerId, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.UtcNow);

	[AllowAnonymous]
	[HttpGet("{username}")]
	public async Task<ActionResult<PlayerStatsDTO?>> GetAsync(string username, [FromQuery] int? comparerId, [FromQuery] int mode = 0, [FromQuery] DateTime? dateMin = null,
		[FromQuery]
		DateTime? dateMax = null) => await _playerStatsService.GetAsync(username, comparerId, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.UtcNow);

	[AllowAnonymous]
	[HttpGet("histogram")]
	public async Task<ActionResult<IDictionary<int, int>>> GetRatingHistogramAsync([FromQuery] int mode = 0) => Ok(await _baseStatsService.GetHistogramAsync(mode));

	[HttpPost("ratingadjustments")]
	public async Task<IActionResult> PostAsync([FromBody] IEnumerable<RatingAdjustmentDTO> postBody)
	{
		await _playerStatsService.BatchInsertAsync(postBody);
		return Ok();
	}

	[HttpDelete("ratingadjustments")]
	public async Task<IActionResult> TruncateAdjustmentsAsync()
	{
		await _playerStatsService.TruncateRatingAdjustmentsAsync();
		return Ok();
	}

	[HttpPost("matchstats")]
	public async Task<IActionResult> PostAsync([FromBody] IEnumerable<PlayerMatchStatsDTO> postBody)
	{
		await _playerStatsService.BatchInsertAsync(postBody);
		return Ok();
	}

	[HttpPost("ratingstats")]
	public async Task<IActionResult> PostAsync([FromBody] IEnumerable<MatchRatingStatsDTO> postBody)
	{
		await _playerStatsService.BatchInsertAsync(postBody);
		return Ok();
	}

	[HttpPost("basestats")]
	public async Task<IActionResult> PostAsync([FromBody] IEnumerable<BaseStatsPostDTO> postBody)
	{
		await _playerStatsService.BatchInsertAsync(postBody);
		return Ok();
	}

	[HttpPost("gamewinrecords")]
	public async Task<IActionResult> PostAsync([FromBody] IEnumerable<GameWinRecordDTO> postBody)
	{
		await _playerStatsService.BatchInsertAsync(postBody);
		return Ok();
	}

	[HttpPost("matchwinrecords")]
	public async Task<IActionResult> PostAsync([FromBody] IEnumerable<MatchWinRecordDTO> postBody)
	{
		await _playerStatsService.BatchInsertAsync(postBody);
		return Ok();
	}

	[HttpDelete]
	public async Task<IActionResult> TruncateAsync()
	{
		await _playerStatsService.TruncateAsync();
		return Ok();
	}
}