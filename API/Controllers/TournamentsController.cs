using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin, System")]
public class TournamentsController : Controller
{
	private readonly IMatchesService _matchesService;

	public TournamentsController(IMatchesService matchesService) { _matchesService = matchesService; }
	
	[HttpGet("verified")]
	public async Task<ActionResult<IEnumerable<Unmapped_VerifiedTournamentDTO>>> GetAllAsync()
	{
		return Ok(await _matchesService.GetAllVerifiedTournamentsAsync());
	}
}