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

	/// <summary>
	///  Validates the currently logged in user's OTR-Access-Token cookie
	/// </summary>
	/// <returns></returns>
	[EndpointSummary("Validates the currently logged in user's OTR-Access-Token cookie")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[HttpGet("validate")]
	public IActionResult ValidateJwt()
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

	[HttpGet]
	public async Task<ActionResult<UserInfoDTO>> GetLoggedInUserAsync()
	{
		int? id = HttpContext.AuthorizedUserIdentity();

		if (!id.HasValue)
		{
			return BadRequest("User's login seems corrupted, couldn't identify osuId.");
		}

		var user = await _userService.GetForPlayerAsync(id.Value);

		if (user == null)
		{
			return NotFound("User not found");
		}

		return Ok(user);
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