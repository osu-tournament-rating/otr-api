using API.DTOs;
using API.Osu.Enums;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class MeController(IUserService userService) : Controller
{
    /// <summary>
    /// Get the currently logged in user
    /// </summary>
    /// <response code="401">If the requester is not properly authenticated</response>
    /// <response code="302">Redirects to `GET` `/users/{id}`</response>
    [HttpGet]
    [Authorize(Roles = OtrClaims.User)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public IActionResult Get()
    {
        var id = User.AuthorizedIdentity();
        if (!id.HasValue)
        {
            return Unauthorized();
        }

        return RedirectToAction("Get", "Users", new { id });
    }

    /// <summary>
    /// Get player stats for the currently logged in user
    /// </summary>
    /// <remarks>
    /// If no ruleset is provided, the player's default is used. <see cref="Ruleset.Standard"/> is used as a fallback.
    /// If a ruleset is provided but the player has no data for it, all optional fields of the response will be null.
    /// <see cref="PlayerStatsDTO.PlayerInfo"/> will always be populated as long as a player is found.
    /// If no date range is provided, gets all stats without considering date
    /// </remarks>
    /// <param name="ruleset">Ruleset to filter for</param>
    /// <param name="dateMin">Filter from earliest date</param>
    /// <param name="dateMax">Filter to latest date</param>
    /// <response code="401">If the requester is not properly authenticated</response>
    /// <response code="302">Redirects to `GET` `/stats/{key}`</response>
    [HttpGet("stats")]
    [Authorize(Roles = OtrClaims.User)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> GetStatsAsync(
        [FromQuery] Ruleset? ruleset = null,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    )
    {
        var userId = HttpContext.AuthorizedUserIdentity();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var playerId = await userService.GetPlayerIdAsync(userId.Value);
        if (!playerId.HasValue)
        {
            return NotFound();
        }

        return RedirectToAction("Get", "Stats", new
        {
            key = playerId.Value,
            ruleset,
            dateMin,
            dateMax
        });
    }

    /// <summary>
    /// Update the ruleset for the currently logged in user
    /// </summary>
    /// <response code="401">If the requester is not properly authenticated</response>
    /// <response code="307">Redirects to `POST` `/users/{id}/settings/ruleset`</response>
    [HttpPost("settings/ruleset")]
    [Authorize(Roles = OtrClaims.User)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status307TemporaryRedirect)]
    public IActionResult UpdateRuleset([FromBody] Ruleset ruleset)
    {
        var userId = HttpContext.AuthorizedUserIdentity();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        return RedirectToActionPreserveMethod("UpdateRuleset", "Users", new { id = userId, ruleset });
    }

    /// <summary>
    /// Sync the ruleset of the currently logged in user to their osu! ruleset
    /// </summary>
    /// <response code="401">If the requester is not properly authenticated</response>
    /// <response code="307">Redirects to `POST` `/users/{id}/settings/ruleset:sync`</response>
    [HttpPost("settings/ruleset:sync")]
    [Authorize(Roles = OtrClaims.User)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status307TemporaryRedirect)]
    public IActionResult SyncRuleset()
    {
        var userId = HttpContext.AuthorizedUserIdentity();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        return RedirectToActionPreserveMethod("SyncRuleset", "Users", new { id = userId });
    }
}
