using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class LeaderboardsController(IPlayerStatsService playerStatsService) : Controller
{
    /// <summary>
    /// Get a leaderboard of players which fit an optional request query
    /// </summary>
    /// <response code="200">Returns the leaderboard</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<LeaderboardDTO>(StatusCodes.Status200OK)]
    public async Task<ActionResult<LeaderboardDTO>> GetAsync(
       [FromQuery] LeaderboardRequestQueryDTO requestQuery
    )
    {
        LeaderboardDTO leaderboard = await playerStatsService.GetLeaderboardAsync(requestQuery);
        return Ok(leaderboard);
    }
}
