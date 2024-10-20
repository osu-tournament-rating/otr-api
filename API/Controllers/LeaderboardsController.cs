using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Authorize(Roles = OtrClaims.Roles.User)]
[Route("api/v{version:apiVersion}/[controller]")]
public class LeaderboardsController(ILeaderboardService leaderboardService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<LeaderboardDTO>> GetAsync(
        [FromQuery] LeaderboardRequestQueryDTO requestQuery
    )
    {
        /*
         * Note:
         *
         * Due to model binding, the query is able to be called as such:
         *
         * ?bronze=true
         * ?grandmaster=false&bronze=true
         * ?ruleset=0&pagesize=25&minrating=500
         *
         * This avoids annoying calls to ".Filter" in the query string (and .Filter.TierFilters for the tier filters)
         */

        LeaderboardDTO leaderboard = await leaderboardService.GetLeaderboardAsync(requestQuery, User.GetSubjectId());
        return Ok(leaderboard);
    }
}
