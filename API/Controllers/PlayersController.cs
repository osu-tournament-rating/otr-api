using API.DTOs;
using API.Entities;
using API.Osu;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[EnableCors]
[Authorize(Roles = "Admin, System")]
[Route("api/[controller]")]
public class PlayersController : Controller
{
	private readonly IPlayerService _playerService;
	public PlayersController(IPlayerService playerService) { _playerService = playerService; }

	[HttpGet("all")]
	public async Task<ActionResult<IEnumerable<Player>?>> GetAllAsync()
	{
		var players = await _playerService.GetAllAsync();
		return Ok(players);
	}

	[AllowAnonymous]
	[HttpGet("{userId:int}/info")]
	public async Task<ActionResult<PlayerInfoDTO?>> GetByUserIdAsync(int userId)
	{
		var player = await _playerService.GetAsync(userId);
		if (player != null)
		{
			return Ok(player);
		}

		return NotFound($"User with id {userId} does not exist");
	}

	[AllowAnonymous]
	[HttpGet("{username}/info")]
	public async Task<ActionResult<PlayerInfoDTO?>> GetByUserIdAsync(string username)
	{
		var player = await _playerService.GetAsync(username);
		if (player != null)
		{
			return Ok(player);
		}

		return NotFound($"User with username {username} does not exist");
	}

	// [HttpGet("{osuId:long}")]
	// public async Task<ActionResult<PlayerDTO?>> Get(long osuId, [FromQuery]int mode = 0, [FromQuery]int offsetDays = -1)
	// {
	// 	string key = $"{osuId}_{offsetDays}_{mode}";
	// 	byte[]? cachedPlayer = await _cache.GetAsync(key);
	// 	var modeEnum = (OsuEnums.Mode)mode;
	// 	if (cachedPlayer != null)
	// 	{
	// 		// Serialize into collection of objects, then filter by offset days.
	// 		var recentCreatedDate = await _ratingsRepository.GetRecentCreatedDate(osuId);
	// 		var cachedObj = JsonConvert.DeserializeObject<PlayerDTO>(Encoding.UTF8.GetString(cachedPlayer));
	// 		if (cachedObj!.BaseStats.MaxBy(x => x.Created)?.Created < recentCreatedDate)
	// 		{
	// 			// Invalidate cache if the player's ratings have been updated since the cache was created.
	// 			await _cache.RemoveAsync(key);
	//
	// 			var newDto = await _playerService.GetByOsuIdAsync(osuId, false, modeEnum, offsetDays);
	// 			if (newDto == null)
	// 			{
	// 				return NotFound($"User with id {osuId} does not exist");
	// 			}
	//
	// 			return Ok(newDto);
	// 		}
	//
	// 		return Ok(cachedObj);
	// 	}
	// 	var data = await _playerService.GetByOsuIdAsync(osuId, false, modeEnum, offsetDays);
	// 	if (data != null)
	// 	{
	// 		return Ok(data);
	// 	}
	//
	// 	return NotFound($"User with id {osuId} does not exist");
	// }

	[HttpGet("{osuId:int}/id")]
	public async Task<ActionResult<int>> GetIdByOsuIdAsync(long osuId)
	{
		int? id = await _playerService.GetIdAsync(osuId);
		if (id != null)
		{
			return Ok(id);
		}

		return NotFound($"User with id {osuId} does not exist");
	}

	[HttpGet("{id}/osuid")]
	public async Task<ActionResult<long>> GetOsuIdAsync(int id)
	{
		long? osuId = await _playerService.GetOsuIdAsync(id);
		if (osuId != null)
		{
			return Ok(osuId);
		}

		return NotFound($"User with id {id} does not exist");
	}

	[HttpGet("ranks/all")]
	public async Task<ActionResult<IEnumerable<PlayerRanksDTO>>> GetAllRanksAsync()
	{
		var ranks = await _playerService.GetAllRanksAsync();
		return Ok(ranks);
	}

	[HttpGet("leaderboard/{mode:int}")]
	public async Task<ActionResult<IEnumerable<PlayerRatingDTO>>> Leaderboard(int gamemode)
	{
		const int LEADERBOARD_LIMIT = 50;
		return Ok(await _playerService.GetTopRatingsAsync(LEADERBOARD_LIMIT, (OsuEnums.Mode)gamemode));
	}

	[HttpGet("id-mapping")]
	public async Task<ActionResult<IEnumerable<Dictionary<long, int>>>> GetIdMappingAsync()
	{
		var mapping = await _playerService.GetIdMappingAsync();
		return Ok(mapping);
	}

	[HttpGet("country-mapping")]
	public async Task<ActionResult<Dictionary<int, string>>> GetCountryMappingAsync()
	{
		var mapping = await _playerService.GetCountryMappingAsync();
		return Ok(mapping);
	}
}