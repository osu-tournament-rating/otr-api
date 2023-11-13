using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[AllowAnonymous]
[EnableCors]
[Route("api/[controller]")]
public class LeaderboardsController : Controller
{
	private readonly ILeaderboardService _leaderboardService;

	public LeaderboardsController(ILeaderboardService leaderboardService)
	{
		_leaderboardService = leaderboardService;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<LeaderboardPlayerInfoDTO>>> GetAsync([FromQuery]LeaderboardRequestQueryDTO requestQuery)
	{
		/**
		 * Note:
		 *
		 * Due to model binding, the query is able to be called as such:
		 *
		 * ?bronze=true
		 * ?grandmaster=false&bronze=true
		 * ?mode=0&pagesize=25&minrating=500&maxwinrate=0.5
		 *
		 * This avoids annoying calls to ".Filter" in the query string (and .Filter.TierFilters for the tier filters)
		 */
		return Ok(await _leaderboardService.GetLeaderboardAsync(requestQuery));
	}
}