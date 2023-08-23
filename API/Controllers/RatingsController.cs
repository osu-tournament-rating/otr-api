using API.Entities;
using API.Services.Interfaces;
using API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text;

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

	[HttpPost("upload")]
	public async Task<IActionResult> UploadRatingsAsync(IFormFile file)
	{
		string fileContent = await file.ReadAsStringAsync();
		var import = JsonConvert.DeserializeObject<IEnumerable<Rating>>(fileContent);
		await _service.UpdateBatchAsync(import!);
		// Format from ratings.json provided by python process
		/**
		 * Example:
		 *
		 * "6578664": {
		        "playerId": 6578664,
		        "osuName": "stefgast13",
		        "muInitial": 1001.4283400310617,
		        "sigmaInitial": 416.6666666666667,
		        "etx": 6.015394690588,
		        "osuRank": 43602.0,
		        "bwsRank": 43602.0,
		        "mode": "Standard",
		        "mu": 1355.5796200805678,
		        "sigma": 154.0321181730947
		    }
		 */
		return Ok();
	}
}