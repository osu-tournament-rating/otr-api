using API.Entities;
using API.Services.Interfaces;
using API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingHistoryController : CrudController<RatingHistory>
{
	private readonly IRatingHistoryService _service;
	private readonly IMatchDataService _matchDataService;

	public RatingHistoryController(ILogger<RatingHistoryController> logger, IRatingHistoryService service,
		IMatchDataService matchDataService) : base(logger, service)
	{
		_service = service;
		_matchDataService = matchDataService;
	}

	[HttpGet("{playerId:int}/all")]
	public async Task<ActionResult<IEnumerable<RatingHistory>>> GetAllForPlayerAsync(int playerId)
	{
		var data = await _service.GetAllForPlayerAsync(playerId);
		if (data.Any())
		{
			return Ok(data);
		}

		return NotFound($"User with id {playerId} does not have any data");
	}
	
	[HttpPost("danger/replace")]
	public async Task<IActionResult> BatchReplaceAsync(IFormFile file)
	{
		string content = await file.ReadAsStringAsync();
		var import = JsonConvert.DeserializeObject<IEnumerable<RatingHistory>>(content);
		
		await _service.ReplaceBatchAsync(import!);
		
		return Ok();
	}
	
	[HttpDelete("danger/truncate")]
	public async Task<IActionResult> TruncateAsync()
	{
		await _service.TruncateAsync();
		return Ok();
	}
	
	[HttpPost("batch")]
	public async Task<IActionResult> BatchAsync(IEnumerable<RatingHistory> histories)
	{
		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		if (histories == null)
		{
			return BadRequest("No data was given or the data was invalid");
		}

		histories = histories.ToList();
		// foreach (var h in histories)
		// {
		// 	if (h.MatchDataId == 0)
		// 	{
		// 		int? matchDataId = await _matchDataService.GetIdAsync(h.PlayerId, h.MatchId, h.GameId);
		// 		if (matchDataId is > 0)
		// 		{
		// 			h.MatchDataId = matchDataId.Value;
		// 		}
		// 		else
		// 		{
		// 			return BadRequest($"Match data with player id {h.PlayerId} and osu match id {h.MatchId} and game id {h.GameId} does not exist");
		// 		}
		// 	}
		// }
		
		var ids = await _matchDataService.GetIdsAsync(histories.Select(x => x.PlayerId).ToList(), histories.Select(x => x.MatchId).ToList(), histories.Select(x => x.GameId).ToList());
		foreach (var h in histories)
		{
			int matchDataId = ids[(h.PlayerId, h.GameId)];
			h.MatchDataId = matchDataId;
		}
		
		await _service.InsertAsync(histories);
		return Ok();
	}
}