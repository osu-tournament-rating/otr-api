using API.Authorization;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Route("api/v{version:apiVersion}/[controller]")]
public class PlayersController(IPlayerService playerService) : Controller
{
    [HttpGet("all")]
    [Authorize(Roles = OtrClaims.System)]
    public async Task<IActionResult> GetAllAsync()
    {
        IEnumerable<PlayerCompactDTO> players = await playerService.GetAllAsync();
        return Ok(players);
    }

    [HttpGet("{key}/info")]
    [Authorize(Roles = $"{OtrClaims.User}, {OtrClaims.Client}")]
    [EndpointSummary("Get player info by versatile search")]
    [EndpointDescription("Get player info searching first by id, then osuId, then username")]
    public async Task<ActionResult<PlayerCompactDTO?>> GetAsync(string key)
    {
        PlayerCompactDTO? info = await playerService.GetVersatileAsync(key);

        if (info == null)
        {
            return NotFound($"User with key {key} does not exist");
        }

        return info;
    }
}
