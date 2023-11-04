using API.DTOs;
using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers;

[ApiController]
[EnableCors]
[Authorize]
[Route("api/[controller]")]
public class MeController : Controller
{
	private readonly IUserService _userService;
	private readonly IPlayerStatsService _playerStatsService;

	public MeController(IPlayerService playerService, IUserService userService, IPlayerStatsService playerStatsService)
	{
		_userService = userService;
		_playerStatsService = playerStatsService;
	}
	
	private long? GetOsuId()
	{
		string? osuId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;
		if (osuId == null)
		{
			return null;
		}
		
		if(!long.TryParse(osuId, out long osuPlayerId))
		{
			return null;
		}

		return osuPlayerId;
	}

	[HttpGet]
	public async Task<ActionResult<User>> GetLoggedInUserAsync()
	{
		long? osuId = GetOsuId();
		
		if(!osuId.HasValue)
		{
			return BadRequest("User's login seems corrupted, couldn't identify osuId.");
		}

		var user = await _userService.GetForPlayerAsync(osuId.Value);
		return Ok(user);
	}
	
	[HttpGet("stats")]
	public async Task<ActionResult<PlayerStatsDTO>> GetStatsAsync([FromQuery]int mode = 0, [FromQuery] DateTime? dateMin = null, [FromQuery] DateTime? dateMax = null)
	{
		int? id = (await GetLoggedInUserAsync()).Value?.Id;
		
		if (!id.HasValue)
		{
			return BadRequest("User is not logged in or id coult not be retreived from logged in user.");
		}

		return await _playerStatsService.GetAsync(id.Value, null, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.UtcNow);
	}
	
	private DateTime FromTime(int offsetDays)
	{
		return offsetDays < 0 ? DateTime.MinValue : DateTime.UtcNow.AddDays(-offsetDays);
	}
}