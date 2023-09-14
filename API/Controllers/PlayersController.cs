using API.DTOs;
using API.Entities;
using API.Osu;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers;

[ApiController]
[Authorize]
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

	[HttpGet("all")]
	public async Task<ActionResult<IEnumerable<Player>?>> GetAllAsync()
	{
		var players = await _service.GetAllAsync();
		return Ok(players);
	}

	[HttpGet("{osuId:long}")]
	public async Task<ActionResult<Player?>> Get(long osuId)
	{
		var currentUser = HttpContext.User;

		if (!currentUser.HasClaim(c => c.Type == JwtRegisteredClaimNames.Name))
		{
			return Unauthorized();
		}
		
		var userId = currentUser.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
		if (userId == null || userId != osuId.ToString())
		{
			return Unauthorized("You are not authorized to view this user's profile");
		}
		
		var data = await _service.GetPlayerDTOByOsuIdAsync(osuId, true);
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