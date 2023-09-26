using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class LeaderboardsController : Controller
{
	private readonly ILeaderboardService _leaderboardService;

	public LeaderboardsController(ILeaderboardService leaderboardService) { _leaderboardService = leaderboardService; }

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Unmapped_LeaderboardDTO>>> GetAsync([FromQuery]int mode, [FromQuery]int page = 0, [FromQuery]int pageSize = 25)
	{
		return Ok(await _leaderboardService.GetLeaderboardAsync(mode, page, pageSize));
	}
}