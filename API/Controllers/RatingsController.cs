using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingsController : CrudController<Rating>
{
	private readonly IRatingsService _service;
	public RatingsController(ILogger<RatingsController> logger, IRatingsService service) : base(logger, service) { _service = service; }

	[HttpGet("{playerId:int}/all")]
	public async Task<ActionResult<IEnumerable<Rating>>> GetAllForPlayerAsync(int playerId)
	{
		var data = await _service.GetAllForPlayerAsync(playerId);
		if (data.Any())
		{
			return Ok(data);
		}

		return NotFound($"User with id {playerId} does not have any data");
	}
}