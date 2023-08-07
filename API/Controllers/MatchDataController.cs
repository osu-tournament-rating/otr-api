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

	[HttpGet("all-filtered")]
	public async Task<ActionResult<IEnumerable<MatchData>>> GetAllFiltered()
	{
		var data = await _service.GetFilteredDataAsync();
		if (data.Any())
		{
			return Ok(data);
		}
		
		return NotFound("No data found");
	}

	[HttpGet("{playerId}/all")]
	public async Task<ActionResult<IEnumerable<MatchData>>> GetAllForPlayerAsync(int playerId)
	{
		var data = await _service.GetAllForPlayerAsync(playerId);
		if (data.Any())
		{
			return Ok(data);
		}

		return NotFound("User does not have any data");
	}
}