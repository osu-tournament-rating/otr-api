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

	public RatingHistoryController(ILogger<RatingHistoryController> logger, IRatingHistoryService service) : base(logger, service)
	{
		_service = service;
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
}