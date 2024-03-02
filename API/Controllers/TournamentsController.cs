using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[EnableCors]
[Route("api/[controller]")]
[Authorize(Roles = "user")]
public class TournamentsController : Controller
{
	private readonly ITournamentsService _service;
	public TournamentsController(ITournamentsService service) { _service = service; }

	[HttpGet]
	[Authorize(Roles = "system")]
	public async Task<IActionResult> GetAsync()
	{
		var res = await _service.GetAllAsync();
		return Ok(res);
	}
    
    [HttpPut]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> PutAsync(TournamentWebSubmissionDTO tournament)
    {
	    var res = await _service.CreateOrUpdateAsync(tournament, true);

	    return Ok(res);
    }
}