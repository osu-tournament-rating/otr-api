using API.Authorization;
using API.DTOs;
using API.Enums;
using API.Services.Interfaces;
using API.Utilities;
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

        var authorizedUserId = User.AuthorizedIdentity();

        if (!authorizedUserId.HasValue && requestQuery.ChartType == LeaderboardChartType.Country)
        {
            return BadRequest("Country leaderboards are only available to logged in users");
        }

        LeaderboardDTO leaderboard = await leaderboardService.GetLeaderboardAsync(requestQuery, authorizedUserId);
        return Ok(leaderboard);
    }
}
