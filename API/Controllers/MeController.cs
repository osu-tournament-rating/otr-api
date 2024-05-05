using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = OtrClaims.User)]
public class MeController(IUserService userService, IPlayerStatsService playerStatsService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly IPlayerStatsService _playerStatsService = playerStatsService;

    /// <summary>
    /// Get the currently logged in user
    /// </summary>
    /// <response code="401">If the requester is not properly authenticated</response>
    /// <response code="404">If a user does not exist</response>
    /// <response code="200">Returns the currently logged in user</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync()
    {
        var id = HttpContext.AuthorizedUserIdentity();
        if (!id.HasValue)
        {
            return Unauthorized();
        }

        UserDTO? user = await _userService.GetAsync(id.Value);
        if (user?.OsuId == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Get player stats for the currently logged in user
    /// </summary>
    /// <remarks>Not specifying a date range will return all player stats</remarks>
    /// <param name="mode">osu! ruleset</param>
    /// <param name="dateMin">Filter from earliest date. If null, earliest possible date</param>
    /// <param name="dateMax">Filter to latest date. If null, latest possible date</param>
    /// <response code="401">If the requester is not properly authenticated</response>
    /// <response code="404">If a user's player entry does not exist</response>
    /// <response code="200">Returns player stats for the currently logged in user</response>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<PlayerStatsDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatsAsync(
        [FromQuery] int mode = 0,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    )
    {
        var userId = HttpContext.AuthorizedUserIdentity();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var playerId = (await _userService.GetAsync(userId.Value))?.PlayerId;
        if (!playerId.HasValue)
        {
            return NotFound();
        }

        PlayerStatsDTO result = await _playerStatsService.GetAsync(
            playerId.Value,
            null,
            mode,
            dateMin ?? DateTime.MinValue,
            dateMax ?? DateTime.UtcNow
        );
        return Ok(result);
    }
}
