using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingHistoryController : Controller
{
	private readonly ILogger<RatingHistoryController> _logger;
	private readonly IRatingHistoryService _service;

	public RatingHistoryController(ILogger<RatingHistoryController> logger, IRatingHistoryService service)
	{
		_logger = logger;
		_service = service;
	}

	[HttpGet("{playerId:int}/all")]
	public async Task<ActionResult<IEnumerable<RatingHistory>>> GetAllForPlayerAsync(int playerId)
	{
		var data = await _service.GetForPlayerAsync(playerId);
		if (data.Any())
		{
			return Ok(data);
		}

		return NotFound($"User with id {playerId} does not have any data");
	}

	[HttpPost("batch")]
	public async Task<IActionResult> BatchReplaceAsync(IEnumerable<RatingHistory> histories)
	{
		await _service.ReplaceBatchAsync(histories);
		return Ok();
	}

	[HttpDelete("danger/truncate")]
	public async Task<IActionResult> TruncateAsync()
	{
		await _service.TruncateAsync();
		return Ok();
	}
}