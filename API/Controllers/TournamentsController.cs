using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TournamentsController : Controller
{
	private readonly IMatchesService _matchesService;

	public TournamentsController(IMatchesService matchesService) { _matchesService = matchesService; }
	
	[HttpGet("verified")]
	[Authorize(Roles = "MatchVerifier, Admin, System")]
	public async Task<ActionResult<IEnumerable<Unmapped_VerifiedTournamentDTO>>> GetAllAsync()
	{
		return Ok(await _matchesService.GetAllVerifiedTournamentsAsync());
	}
}