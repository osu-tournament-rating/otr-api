using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BeatmapsController : Controller
{
	private readonly ILogger<BeatmapsController> _logger;
	private readonly IBeatmapService _beatmapService;
	public BeatmapsController(ILogger<BeatmapsController> logger, IBeatmapService beatmapService)
	{
		_logger = logger;
		_beatmapService = beatmapService;
	}
	
	[HttpGet("all")]
	public async Task<ActionResult<IEnumerable<Beatmap>>> GetAllAsync()
	{
		return Ok(await _beatmapService.GetAllAsync());
	}
}