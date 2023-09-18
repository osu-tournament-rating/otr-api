using API.DTOs;
using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin, System")]
public class RatingHistoryController : Controller
{
	private readonly ILogger<RatingHistoryController> _logger;
	private readonly IRatingHistoryService _service;

	public RatingHistoryController(ILogger<RatingHistoryController> logger, IRatingHistoryService service)
	{
		_logger = logger;
		_service = service;
	}

	[HttpGet("{osuPlayerId:long}/all")]
	public async Task<ActionResult<IEnumerable<RatingHistory>>> GetAllForPlayerAsync(long osuPlayerId)
	{
		var data = await _service.GetForPlayerAsync(osuPlayerId, DateTime.MinValue);
		if (data.Any())
		{
			return Ok(data);
		}

		return NotFound($"User with id {osuPlayerId} does not have any data");
	}

	[HttpPost("batch")]
	public async Task<IActionResult> BatchReplaceAsync(IEnumerable<RatingHistoryDTO> histories)
	{
		int? result = await _service.BatchInsertAsync(histories);
		if (result > 0)
		{
			return Ok();
		}

		return StatusCode(500, "Failed to update ratings");
	}

	[HttpDelete("danger/truncate")]
	public async Task<IActionResult> TruncateAsync()
	{
		await _service.TruncateAsync();
		return Ok();
	}
}