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
		return Ok(await _leaderboardService.GetLeaderboardAsync(requestQuery));
	}
}