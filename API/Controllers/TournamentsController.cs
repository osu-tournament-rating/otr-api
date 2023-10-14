using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TournamentsController : Controller
{
	private readonly ITournamentsService _tournamentsService;
	public TournamentsController(ITournamentsService tournamentsService) { _tournamentsService = tournamentsService; }

	[HttpPost("populate")]
	[EndpointSummary("Takes existing data from known matches, inserts them into the tournaments table, and links tournaments to matches.")]
	public async Task<IActionResult> PopulateAsync()
	{
		await _tournamentsService.PopulateAndLinkAsync();
		return NoContent();
	}
}