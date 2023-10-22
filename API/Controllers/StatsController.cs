using API.DTOs;
using API.Entities;
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
	private readonly IPlayerStatisticsService _statisticsService;

	public StatsController(ILogger<StatsController> logger, IDistributedCache cache, IPlayerStatisticsService statisticsService)
	{
		_logger = logger;
		_cache = cache;
		_statisticsService = statisticsService;
	}
	
	[HttpGet("{osuId:long}")]
	[AllowAnonymous]
	public async Task<ActionResult<PlayerStatisticsDTO>> GetAsync(long osuId, [FromQuery]int mode = 0, [FromQuery] DateTime? dateMin = null, [FromQuery] DateTime? dateMax = null)
	{
		var result = await _statisticsService.GetAsync(osuId, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.MaxValue);
		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> PostAsync(PlayerMatchStatistics postBody)
	{
		await _statisticsService.InsertAsync(postBody);
		return Ok();
	}

	[HttpDelete]
	public async Task<IActionResult> TruncateAsync()
	{
		await _statisticsService.TruncateAsync();
		return Ok();
	}
}