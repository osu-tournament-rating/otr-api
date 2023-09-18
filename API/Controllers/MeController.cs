using API.DTOs;
using API.Osu;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MeController : Controller
{
	private readonly IPlayerService _playerService;
	private readonly IRatingsService _ratingsService;
	private readonly IDistributedCache _cache;
	public MeController(IPlayerService playerService, IRatingsService ratingsService, IDistributedCache cache)
	{
		_playerService = playerService;
		_ratingsService = ratingsService;
		_cache = cache;
	}

	[HttpGet]
	public async Task<ActionResult<PlayerDTO>> GetAsync([FromQuery]int offsetDays = -1, [FromQuery]int mode = 0)
	{
		if(!HttpContext.User.HasClaim(x => x.Type == JwtRegisteredClaimNames.Name))
		{
			return Unauthorized();
		}
		
		string? osuId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;
		if (osuId == null)
		{
			return Unauthorized();
		}
		
		if(!long.TryParse(osuId, out long osuPlayerId))
		{
			return BadRequest("User's login seems corrupted.");
		}

		string osuIdKey = HttpContext.User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value;
		string key = $"{osuIdKey}_{offsetDays}_{mode}";
		byte[]? cachedPlayer = await _cache.GetAsync(key);
		if (cachedPlayer != null)
		{
			// Serialize into collection of objects, then filter by offset days.
			var recentCreatedDate = await _ratingsService.GetRecentCreatedDate(osuPlayerId);
			var cachedObj = JsonConvert.DeserializeObject<PlayerDTO>(Encoding.UTF8.GetString(cachedPlayer));
			if (cachedObj!.Ratings.MaxBy(x => x.Created)?.Created < recentCreatedDate)
			{
				// Invalidate cache if the player's ratings have been updated since the cache was created.
				await _cache.RemoveAsync(key);

				var newDto = await _playerService.GetPlayerDTOByOsuIdAsync(osuPlayerId, true, (OsuEnums.Mode) mode, offsetDays);
				if (newDto == null)
				{
					return NotFound();
				}

				await _cache.SetStringAsync(key, JsonConvert.SerializeObject(newDto));
				return Ok(newDto);
			}

			return Ok(cachedObj);
		}
		else
		{
			var player = await _playerService.GetPlayerDTOByOsuIdAsync(osuPlayerId, true, (OsuEnums.Mode) mode, offsetDays);
			if (player == null)
			{
				return NotFound();
			}
		
			await _cache.SetStringAsync(key, JsonConvert.SerializeObject(player));
			return Ok(player);
		}
	}
}