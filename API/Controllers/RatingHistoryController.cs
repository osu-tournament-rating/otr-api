using API.Entities;
using API.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingHistoryController : CrudController<RatingHistory>
{
	private readonly RatingHistoryService _service;
	public RatingHistoryController(ILogger<RatingHistoryController> logger, RatingHistoryService service) : base(logger, service) { _service = service; }

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
}