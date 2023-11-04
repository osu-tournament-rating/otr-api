using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace API.Controllers;

[ApiController]
[EnableCors]
[Route("api/[controller]")]
[Authorize(Roles = "Admin, System")]
public class StatsController : Controller
{
	private readonly ILogger<StatsController> _logger;
	private readonly IDistributedCache _cache;
	private readonly IPlayerStatsService _playerStatsService;

	public StatsController(ILogger<StatsController> logger, IDistributedCache cache, IPlayerStatsService playerStatsService)
	{
		_logger = logger;
		_cache = cache;
		_playerStatsService = playerStatsService;
	}
	
	[Authorize]
	[HttpGet("{playerId:long}")]
	public async Task<ActionResult<PlayerStatsDTO>> GetAsync(int playerId, [FromQuery]int? comparerId, [FromQuery]int mode = 0, [FromQuery] DateTime? dateMin = null, [FromQuery] DateTime? dateMax = null)
	{
		return await _playerStatsService.GetAsync(playerId, comparerId, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.UtcNow);
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
	
	[HttpDelete]
	public async Task<IActionResult> TruncateAsync()
	{
		await _playerStatsService.TruncateAsync();
		return Ok();
	}
}