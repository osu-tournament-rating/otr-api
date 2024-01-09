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

	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> GetAsync()
	{
		var res = await _service.GetAllAsync();
		return Ok(res);
	}
}