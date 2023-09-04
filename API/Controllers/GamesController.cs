using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : Controller
{
	private readonly IGamesService _gamesService;

	public GamesController(ILogger<GamesController> logger, IGamesService gamesService) { _gamesService = gamesService; }
	
	[HttpGet("match-from-gameid/{gameId:int}")]
	public async Task<ActionResult<Match?>> GetMatchByGameIdAsync(int gameId)
	{
		var match = await _gamesService.GetMatchByGameIdAsync(gameId);
		if (match == null)
		{
			return NotFound("No matching matchId in the database.");
		}

		return Ok(match);
	}
}