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

		var historiesList = histories.ToList();
		var ids = await _matchDataService.GetIdsAsync(historiesList.Select(x => x.PlayerId).ToList(), historiesList.Select(x => x.MatchId).ToList(), historiesList.Select(x => x.GameId).ToList());
		foreach (var h in historiesList)
		{
			try
			{
				int matchDataId = ids[(h.PlayerId, h.GameId)];
				h.MatchDataId = matchDataId;
			}
			catch (Exception e)
			{
				historiesList.Remove(h);
				Logger.LogError(e, "Match data with player id {PlayerId} and osu match id {MatchId} and game id {GameId} does not exist", h.PlayerId, h.MatchId, h.GameId);
			}
		}
		
		await _service.InsertAsync(historiesList);
		return Ok();
	}
}