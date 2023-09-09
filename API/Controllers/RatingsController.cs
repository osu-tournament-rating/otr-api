using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingsController : Controller
{
	private readonly ILogger<RatingsController> _logger;
	private readonly IRatingsService _service;

	public RatingsController(ILogger<RatingsController> logger, IRatingsService service)
	{
		_logger = logger;
		_service = service;
	}

	[HttpGet("all")]
	public async Task<ActionResult<IEnumerable<Rating>?>> GetAllAsync()
	{
		var ratings = await _service.GetAllAsync();
		return Ok(ratings);
	}

	[HttpGet("{osuPlayerId:long}")]
	public async Task<ActionResult<Rating>> GetForPlayerAsync(long osuPlayerId)
	{
		var data = await _service.GetForPlayerAsync(osuPlayerId);
		if (!data.IsNullOrEmpty())
		{
			return Ok(data);
		}

		return NotFound($"User with id {osuPlayerId} does not have any data");
	}

	[HttpPut("{playerId:int}/update")]
	public async Task<ActionResult> UpdateForPlayerAsync(int playerId, [FromBody] Rating rating)
	{
		if (playerId != rating.PlayerId)
		{
			return BadRequest($"Player id {rating.PlayerId} in body does not match player id {playerId} in path");
		}

		int? result = await _service.InsertOrUpdateForPlayerAsync(playerId, rating);
		if (result > 0)
		{
			return Ok();
		}

		return StatusCode(500, $"Failed to update rating for player {playerId}");
	}

	[HttpPost("batch")]
	public async Task<ActionResult> BatchInsertOrUpdateAsync([FromBody] IEnumerable<Rating> ratings)
	{
		int? result = await _service.BatchInsertAsync(ratings);
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