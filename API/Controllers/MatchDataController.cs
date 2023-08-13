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

	[HttpGet("{id:int}/all")]
	public async Task<ActionResult<IEnumerable<MatchData>>> GetAllForPlayerAsync(int id)
	{
		var data = await _service.GetAllForPlayerAsync(id);
		if (data.Any())
		{
			return Ok(data);
		}

		return NotFound($"User with id {id} does not have any data");
	}
}