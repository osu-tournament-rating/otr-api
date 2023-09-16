using API.DTOs;
using API.Entities;
using API.Osu;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class PlayersController : Controller
{
	private readonly ILogger<PlayersController> _logger;
	private readonly IPlayerService _service;

	public PlayersController(ILogger<PlayersController> logger, IPlayerService service)
	{
		_logger = logger;
		_service = service;
	}

	[HttpGet("stats/{osuId:long}")]
	public async Task<IActionResult> GetPlayerStatsAsync(long osuId, [FromQuery]int mode = 0)
	{
		var stats = await _service.GetPlayerStatisticsAsync(osuId, (OsuEnums.Mode) mode);
		return Ok(stats);
	}

	[HttpGet("all")]
	public async Task<ActionResult<IEnumerable<Player>?>> GetAllAsync()
	{
		var players = await _service.GetAllAsync();
		return Ok(players);
	}

	[HttpGet("{osuId:long}")]
	public async Task<ActionResult<PlayerDTO?>> Get(long osuId, [FromQuery]int offsetDays = -1)
	{
		var data = await _service.GetPlayerDTOByOsuIdAsync(osuId, true, offsetDays);
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