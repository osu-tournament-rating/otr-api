using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin, System")]
public class StatsController : Controller
{
	private readonly ILogger<StatsController> _logger;
	private readonly IDistributedCache _cache;
	private readonly IPlayerStatisticsService _playerStatsService;

	public StatsController(ILogger<StatsController> logger, IDistributedCache cache, IPlayerStatisticsService playerStatsService)
	{
		_logger = logger;
		_cache = cache;
		_playerStatsService = playerStatsService;
	}
	
	// [Authorize]
	[HttpGet("{osuId:long}")]
	public async Task<ActionResult<PlayerStatisticsDTO>> GetAsync(long osuId, [FromQuery]int mode = 0, [FromQuery] DateTime? dateMin = null, [FromQuery] DateTime? dateMax = null)
	{
		var result = await _playerStatsService.GetAsync(osuId, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.UtcNow);
		return Ok(result);
	}

	[HttpPost("matchstats")]
	public async Task<IActionResult> PostAsync([FromBody] IEnumerable<PlayerMatchStatisticsDTO> postBody)
	{
		await _playerStatsService.BatchInsertAsync(postBody);
		return Ok();
	}
	
	[HttpPost("ratingstats")]
	public async Task<IActionResult> PostAsync([FromBody] IEnumerable<MatchRatingStatisticsDTO> postBody)
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