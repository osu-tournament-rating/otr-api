using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
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

	public MeController(IUserService userService, IPlayerStatsService playerStatsService)
	{
		_userService = userService;
		_playerStatsService = playerStatsService;
	}

	private bool IsWhitelisted(long osuId)
	{
		var whitelist = new long[]
		{
			11482346,
			8191845,
			11557554,
			4001304,
			6892711,
			7153533,
			3958619,
			6701656,
			1797189,
			7802400,
			11255340,
			13175102,
			11955929,
			11292327,
			18618027
		};

		return whitelist.Contains(osuId);
	}
	
	[HttpGet]
	[EndpointSummary("Gets the logged in user's information, if they exist")]
	[ProducesResponseType<UserInfoDTO>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<UserInfoDTO>> GetLoggedInUserAsync()
	{
		int? id = HttpContext.AuthorizedUserIdentity();

		if (!id.HasValue)
		{
			return BadRequest("User is not logged in.");
		}

		var user = await _userService.GetForPlayerAsync(id.Value);
		if (user?.OsuId == null)
		{
			return NotFound("User not found");
		}

		if (!IsWhitelisted(user.OsuId.Value))
		{
			return Unauthorized("User is not whitelisted");
		}

		return Ok(user);
	}

	/// <summary>
	///  Validates the currently logged in user's OTR-Access-Token cookie
	/// </summary>
	/// <returns></returns>
	[HttpGet("validate")]
	[EndpointSummary("Validates the currently logged in user's OTR-Access-Token cookie")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ValidateJwt()
	{
		if (!HttpContext.Request.Cookies.ContainsKey("OTR-Access-Token"))
		{
			return BadRequest("No access token cookie found.");
		}

		string? token = HttpContext.Request.Cookies["OTR-Access-Token"];
		if (string.IsNullOrEmpty(token))
		{
			return BadRequest("Cookie found, but was null or empty.");
		}

		var loggedInUser = await GetLoggedInUserAsync();
		if (loggedInUser.Result is UnauthorizedResult)
		{
			return Unauthorized("User is not whitelisted");
		}

		var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
		var expDate = jwtToken.ValidTo;
		if (expDate < DateTime.UtcNow)
		{
			return NoContent();
		}
		
		return Ok();
	}

	private int? GetId()
	{
		string? id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;
		if (id == null)
		{
			return null;
		}

		if (!int.TryParse(id, out int idInt))
		{
			return null;
		}

		return idInt;
	}

	[HttpGet("stats")]
	public async Task<ActionResult<PlayerStatsDTO>> GetStatsAsync([FromQuery] int mode = 0, [FromQuery] DateTime? dateMin = null, [FromQuery] DateTime? dateMax = null)
	{
		int? id = GetId();

		if (!id.HasValue)
		{
			return BadRequest("User is not logged in or id could not be retreived from logged in user.");
		}

		return await _playerStatsService.GetAsync(id.Value, null, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.UtcNow);
	}
}