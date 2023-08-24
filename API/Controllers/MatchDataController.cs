using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchDataController : CrudController<MatchData>
{
	private readonly IMatchDataService _service;
	public MatchDataController(ILogger<MatchDataController> logger, IMatchDataService service) : base(logger, service) { _service = service; }

	[HttpGet("{playerId:int}/all")]
	public async Task<ActionResult<IEnumerable<MatchData>>> GetAllForPlayerAsync(int playerId)
	{
		var data = await _service.GetAllForPlayerAsync(playerId);
		if (data.Any())
		{
			return Ok(data);
		}

		return NotFound($"User with id {playerId} does not have any data");
	}
	
	[HttpGet("{playerId:int}/{osuMatchId:long}/{gameId:long}/id")]
	public async Task<ActionResult<int>> GetIdAsync(int playerId, long osuMatchId, long gameId)
	{
		int? id = await _service.GetIdAsync(playerId, osuMatchId, gameId);
		if (id != null)
		{
			return Ok(id);
		}

		return NotFound($"Match data with player id {playerId} and osu match id {osuMatchId} and game id {gameId} does not exist");
	}
}