using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[EnableCors]
[Route("api/v1/[controller]")]
[Authorize]
public class TournamentsController : Controller
{
	private readonly ITournamentsService _service;
	public TournamentsController(ITournamentsService service) { _service = service; }

	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> GetAsync()
	{
		var res = await _service.GetAllAsync();
		return Ok(res);
	}
    
    [HttpPut]
    [AllowAnonymous]
    public async Task<IActionResult> PutAsync(TournamentWebSubmissionDTO tournament)
    {
	    var res = await _service.CreateOrUpdateAsync(tournament, true);

	    return Ok(res);
    }
}