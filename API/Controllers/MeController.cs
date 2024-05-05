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
public class MeController(IUserService userService) : Controller
{
    /// <summary>
    /// Get the currently logged in user
    /// </summary>
    /// <response code="401">If the requester is not properly authenticated</response>
    /// <response code="302">Redirects to `GET` `/users/{id}`</response>
    [HttpGet]
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
    /// <remarks>Not specifying a date range will return all player stats</remarks>
    /// <param name="mode">osu! ruleset. If null, osu! Standard is used</param>
    /// <param name="dateMin">Filter from earliest date. If null, earliest possible date</param>
    /// <param name="dateMax">Filter to latest date. If null, latest possible date</param>
    /// <response code="401">If the requester is not properly authenticated</response>
    /// <response code="404">If a user's player entry does not exist</response>
    /// <response code="302">Redirects to `GET` `/stats/{id}`</response>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status302Found)]
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

        var playerId = (await userService.GetAsync(userId.Value))?.PlayerId;
        if (!playerId.HasValue)
        {
            return NotFound();
        }

        return RedirectToAction("GetByUsername", "Stats", new
        {
            id = playerId,
            mode,
            dateMin,
            dateMax
        });
    }
}
