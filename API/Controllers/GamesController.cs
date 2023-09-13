using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : Controller
{
	private readonly IGamesService _gamesService;
	public GamesController(ILogger<GamesController> logger, IGamesService gamesService) { _gamesService = gamesService; }

	[HttpPost("recalc-postmod-sr")]
	public async Task<ActionResult> RecalcPostModSr()
	{
		await _gamesService.UpdateAllPostModSrsAsync();
		return Ok();
	}
}