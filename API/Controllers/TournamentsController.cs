using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "user")]
[Authorize(Roles = "whitelist")]
public class TournamentsController(ITournamentsService service) : Controller
{
    private readonly ITournamentsService _service = service;

    [HttpGet]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> GetAsync()
    {
        IEnumerable<TournamentDTO> res = await _service.GetAllAsync();
        return Ok(res);
    }

    [HttpPut]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> PutAsync(TournamentWebSubmissionDTO tournament)
    {
        TournamentDTO res = await _service.CreateOrUpdateAsync(tournament, true);

        return Ok(res);
    }
}
