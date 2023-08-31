using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchDataController : CrudController<PlayerMatchData>
{
	private readonly IMatchDataService _service;
	public MatchDataController(ILogger<MatchDataController> logger, IMatchDataService service) : base(logger, service) { _service = service; }

	[HttpGet("{playerId:int}/all")]
	public async Task<ActionResult<IEnumerable<PlayerMatchData>>> GetAllForPlayerAsync(int playerId)
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
	
	[HttpGet("ids")]
	public async Task<ActionResult<IEnumerable<int>>> GetIdsAsync([FromBody] IdsPostBody postBody)
	{
		var ids = await _service.GetIdsAsync(postBody.PlayerIds, postBody.OsuMatchIds, postBody.GameIds);
		if (ids.Any())
		{
			return Ok(ids);
		}

		return NotFound();
	}
}

public class IdsPostBody
{
	public IEnumerable<int> PlayerIds { get; set; }
	public IEnumerable<long> OsuMatchIds { get; set; }
	public IEnumerable<long> GameIds { get; set; }
}