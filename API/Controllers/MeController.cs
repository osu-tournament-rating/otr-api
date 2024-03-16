using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "user")]
[Authorize(Roles = "whitelist")]
public class MeController(IUserService userService, IPlayerStatsService playerStatsService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly IPlayerStatsService _playerStatsService = playerStatsService;

    [HttpGet]
    [EndpointSummary("Gets the logged in user's information, if they exist")]
    [ProducesResponseType<UserInfoDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLoggedInUserAsync()
    {
        var id = HttpContext.AuthorizedUserIdentity();

        if (!id.HasValue)
        {
            return BadRequest("Authorization invalid.");
        }

        UserInfoDTO? user = await _userService.GetAsync(id.Value);
        if (user?.OsuId == null)
        {
            return NotFound("User not found");
        }

        return Ok(user);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<PlayerStatsDTO>> GetStatsAsync(
        [FromQuery] int mode = 0,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    )
    {
        var userId = HttpContext.AuthorizedUserIdentity();

        if (!userId.HasValue)
        {
            return BadRequest("User is not logged in or id could not be retrieved from logged in user.");
        }

        var playerId = (await _userService.GetAsync(userId.Value))?.Id;

        if (!playerId.HasValue)
        {
            return BadRequest("Unidentifiable user (unable to discern playerId).");
        }

        return await _playerStatsService.GetAsync(
            playerId.Value,
            null,
            mode,
            dateMin ?? DateTime.MinValue,
            dateMax ?? DateTime.UtcNow
        );
    }
}
