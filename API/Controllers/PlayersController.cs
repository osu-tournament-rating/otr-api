using API.DTOs;
using API.Entities;
using API.Osu;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin, System")]
[Route("api/[controller]")]
public class PlayersController : Controller
{
	private readonly ILogger<PlayersController> _logger;
	private readonly IDistributedCache _cache;
	private readonly IPlayerService _service;
	private readonly IRatingsService _ratingsService;

	public PlayersController(ILogger<PlayersController> logger, IDistributedCache cache, 
		IPlayerService service, IRatingsService ratingsService)
	{
		_logger = logger;
		_cache = cache;
		_service = service;
		_ratingsService = ratingsService;
	}

	[HttpGet("stats/{osuId:long}")]
	public async Task<IActionResult> GetPlayerStatsAsync(long osuId, [FromQuery]int mode = 0)
	{
		var stats = await _service.GetVerifiedPlayerStatisticsAsync(osuId, (OsuEnums.Mode) mode);
		return Ok(stats);
	}

	[HttpGet("all")]
	public async Task<ActionResult<IEnumerable<Player>?>> GetAllAsync()
	{
		var players = await _service.GetAllAsync();
		return Ok(players);
	}

	[HttpGet("{osuId:long}")]
	public async Task<ActionResult<PlayerDTO?>> Get(long osuId, [FromQuery]int mode = 0, [FromQuery]int offsetDays = -1)
	{
		string key = $"{osuId}_{offsetDays}_{mode}";
		byte[]? cachedPlayer = await _cache.GetAsync(key);
		var modeEnum = (OsuEnums.Mode)mode;
		if (cachedPlayer != null)
		{
			// Serialize into collection of objects, then filter by offset days.
			var recentCreatedDate = await _ratingsService.GetRecentCreatedDate(osuId);
			var cachedObj = JsonConvert.DeserializeObject<PlayerDTO>(Encoding.UTF8.GetString(cachedPlayer));
			if (cachedObj!.Ratings.MaxBy(x => x.Created)?.Created < recentCreatedDate)
			{
				// Invalidate cache if the player's ratings have been updated since the cache was created.
				await _cache.RemoveAsync(key);

				var newDto = await _service.GetPlayerDTOByOsuIdAsync(osuId, false, modeEnum, offsetDays);
				if (newDto == null)
				{
					return NotFound($"User with id {osuId} does not exist");
				}

				return Ok(newDto);
			}

			return Ok(cachedObj);
		}
		var data = await _service.GetPlayerDTOByOsuIdAsync(osuId, false, modeEnum, offsetDays);
		if (data != null)
		{
			return Ok(data);
		}

		return NotFound($"User with id {osuId} does not exist");
	}

	[HttpGet("{osuId:int}/id")]
	public async Task<ActionResult<int>> GetIdByOsuIdAsync(long osuId)
	{
		int id = await _service.GetIdByOsuIdAsync(osuId);
		if (id != 0)
		{
			return Ok(id);
		}

		return NotFound($"User with id {osuId} does not exist");
	}

	[HttpGet("{id}/osuid")]
	public async Task<ActionResult<long>> GetOsuIdByIdAsync(int id)
	{
		long osuId = await _service.GetOsuIdByIdAsync(id);
		if (osuId != 0)
		{
			return Ok(osuId);
		}

		return NotFound($"User with id {id} does not exist");
	}
	
	[HttpGet("ranks/all")]
	public async Task<ActionResult<IEnumerable<PlayerRanksDTO>>> GetAllRanksAsync()
	{
		var ranks = await _service.GetAllRanksAsync();
		return Ok(ranks);
	}
	
	[HttpGet("leaderboard/{mode:int}")]
	public async Task<ActionResult<IEnumerable<Unmapped_PlayerRatingDTO>>> Leaderboard(int gamemode)
	{
		const int LEADERBOARD_LIMIT = 50;
		return Ok(await _service.GetTopRatingsAsync(LEADERBOARD_LIMIT, (OsuEnums.Mode) gamemode));
	}
}