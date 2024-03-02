using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Asp.Versioning;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "user")]
public class MeController : Controller
{
	private readonly IUserService _userService;
	private readonly IPlayerStatsService _playerStatsService;

	public MeController(IUserService userService, IPlayerStatsService playerStatsService)
	{
		_userService = userService;
		_playerStatsService = playerStatsService;
	}

	// Will remove once all users are whitelisted in the database
	// private bool IsWhitelisted(long osuId)
	// {
	// 	var whitelist = new long[]
	// 	{
	// 		11482346,
	// 		8191845,
	// 		11557554,
	// 		4001304,
	// 		6892711,
	// 		7153533,
	// 		3958619,
	// 		6701656,
	// 		1797189,
	// 		7802400,
	// 		11255340,
	// 		13175102,
	// 		11955929,
	// 		11292327
	// 	};
	//
	// 	return whitelist.Contains(osuId);
	// }

	[HttpGet]
	[EndpointSummary("Gets the logged in user's information, if they exist")]
	[ProducesResponseType<UserInfoDTO>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetLoggedInUserAsync()
	{
		int? id = HttpContext.AuthorizedUserIdentity();

		if (!id.HasValue)
		{
			return BadRequest("User is not logged in.");
		}

		var user = await _userService.GetAsync(id.Value);
		if (user?.OsuId == null)
		{
			return NotFound("User not found");
		}

		return Ok(user);
	}

	/// <summary>
	///  Validates the currently logged in user's OTR-Access-Token cookie
	/// </summary>
	/// <returns></returns>
	[HttpGet("validate")]
	[Authorize(Roles = "whitelist")]
	[EndpointSummary("Validates the currently logged in user has permissions to access the website.")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public IActionResult ValidateJwt()
	{
		// Middleware will return 403 if the user does not
		// have the correct roles
		return NoContent();
	}

	[HttpGet("stats")]
	public async Task<ActionResult<PlayerStatsDTO>> GetStatsAsync([FromQuery] int mode = 0, [FromQuery] DateTime? dateMin = null, [FromQuery] DateTime? dateMax = null)
	{
		int? userId = HttpContext.AuthorizedUserIdentity();

		if (!userId.HasValue)
		{
			return BadRequest("User is not logged in or id could not be retreived from logged in user.");
		}

		int? playerId = (await _userService.GetAsync(userId.Value))?.Id;

		if (!playerId.HasValue)
		{
			return BadRequest("Unidentifiable user (unable to discern playerId).");
		}

		return await _playerStatsService.GetAsync(playerId.Value, null, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.UtcNow);
	}
}