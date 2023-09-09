using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : Controller
{
	private readonly IMatchesService _matchesService;
	public GamesController(ILogger<GamesController> logger, IMatchesService matchesService) { _matchesService = matchesService; }

	// Probably a useless endpoint
	[HttpGet("{osuGameId:long}/match")]
	public async Task<ActionResult<Match?>> GetMatchByGameIdAsync(long osuGameId)
	{
		var match = await _matchesService.GetByOsuGameIdAsync(osuGameId);
		if (match == null)
		{
			return NotFound("No matching matchId in the database.");
		}

		return Ok(match);
	}
}