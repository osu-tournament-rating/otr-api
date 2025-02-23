using System.ComponentModel.DataAnnotations;
using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using Asp.Versioning;
using Database.Enums;
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
    /// <response code="302">Redirects to `GET` `/users/{id}`</response>
    /// <response code="200">Returns the currently logged in user</response>
    [HttpGet]
    [Authorize(Roles = OtrClaims.Roles.User)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType<UserDTO>(StatusCodes.Status200OK)]
    public IActionResult Get() =>
        RedirectToAction("Get", "Users", new { id = User.GetSubjectId() });

    /// <summary>
    /// Get player stats for the currently logged in user
    /// </summary>
    /// <remarks>
    /// If no ruleset is provided, the player's default is used. <see cref="Ruleset.Osu"/> is used as a fallback.
    /// If a ruleset is provided but the player has no data for it, all optional fields of the response will be null.
    /// <see cref="PlayerDashboardStatsDTO.PlayerInfo"/> will always be populated as long as a player is found.
    /// If no date range is provided, gets all stats without considering date
    /// </remarks>
    /// <param name="ruleset">Ruleset to filter for</param>
    /// <param name="dateMin">Filter from earliest date</param>
    /// <param name="dateMax">Filter to latest date</param>
    /// <response code="302">Redirects to `GET` `/players/{key}/stats`</response>
    /// <response code="200">Returns the currently logged in user's player stats</response>
    [HttpGet("stats")]
    [Authorize(Roles = OtrClaims.Roles.User)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType<PlayerDashboardStatsDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatsAsync(
        [FromQuery] Ruleset? ruleset = null,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    )
    {
        var playerId = await userService.GetPlayerIdAsync(User.GetSubjectId());
        if (!playerId.HasValue)
        {
            return NotFound();
        }

        return RedirectToAction("GetStats", "Players", new
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
    /// <response code="308">Redirects to `PATCH` `/users/{id}/settings/ruleset`</response>
    /// <response code="400">The operation was not successful</response>
    /// <response code="200">The operation was successful</response>
    [HttpPatch("settings/ruleset")]
    [Authorize(Roles = OtrClaims.Roles.User)]
    [ProducesResponseType(StatusCodes.Status308PermanentRedirect)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult UpdateRuleset([FromBody][Required] Ruleset ruleset) =>
        RedirectToActionPermanentPreserveMethod(
            nameof(UsersController.UpdateRulesetAsync),
            nameof(UsersController),
            new { id = User.GetSubjectId(), ruleset }
        );

    /// <summary>
    /// Sync the ruleset of the currently logged in user to their osu! ruleset
    /// </summary>
    /// <response code="307">Redirects to `POST` `/users/{id}/settings/ruleset:sync`</response>
    /// <response code="400">The operation was not successful</response>
    /// <response code="200">The operation was successful</response>
    [HttpPost("settings/ruleset:sync")]
    [Authorize(Roles = OtrClaims.Roles.User)]
    [ProducesResponseType(StatusCodes.Status308PermanentRedirect)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult SyncRuleset() =>
        RedirectToActionPermanentPreserveMethod(
            nameof(UsersController.SyncRulesetAsync),
            nameof(UsersController),
            new { id = User.GetSubjectId() }
        );
}
