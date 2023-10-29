using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[EnableCors]
[Route("api/[controller]")]
[Authorize]
public class TournamentsController : Controller
{
	private readonly ITournamentsService _service;
	public TournamentsController(ITournamentsService service) { _service = service; }

	[HttpPost("populate")]
	[EndpointSummary("Takes existing data from known matches, inserts them into the tournaments table, and links tournaments to matches.")]
	public async Task<IActionResult> PopulateAsync()
	{
		await _service.PopulateAndLinkAsync();
		return NoContent();
	}
}