using API.DTOs;
using API.Osu;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Authorize(Roles = "user, whitelist")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PlayersController(IPlayerService playerService) : Controller
{
    private readonly IPlayerService _playerService = playerService;

    [HttpGet("all")]
    [Authorize(Roles = "system")]
    public async Task<IActionResult> GetAllAsync()
    {
        IEnumerable<PlayerDTO> players = await _playerService.GetAllAsync();
        return Ok(players);
    }

    [HttpGet("{key}/info")]
    [EndpointSummary("Get player info by versatile search")]
    [EnpointDescription("Get player info searching first by id, then osuId, then username")]
    public async Task<ActionResult<PlayerInfoDTO?>> GetAsync(string key)
    {
        var info = await _playerService.GetVersatileAsync(key);

        if (info == null)
        {
            return NotFound($"User with key {key} does not exist");
        }

        return info;
    }

    [HttpGet("ranks/all")]
    [Authorize(Roles = "system")]
    public async Task<ActionResult<IEnumerable<PlayerRanksDTO>>> GetAllRanksAsync()
    {
        IEnumerable<PlayerRanksDTO> ranks = await _playerService.GetAllRanksAsync();
        return Ok(ranks);
    }

    [HttpGet("id-mapping")]
    [Authorize(Roles = "system")]
    public async Task<ActionResult<IEnumerable<PlayerIdMappingDTO>>> GetIdMappingAsync()
    {
        IEnumerable<PlayerIdMappingDTO> mapping = await _playerService.GetIdMappingAsync();
        return Ok(mapping);
    }

    [HttpGet("country-mapping")]
    [Authorize(Roles = "system")]
    [ProducesResponseType<IEnumerable<PlayerCountryMappingDTO>>(StatusCodes.Status200OK)]
    [EndpointSummary(
        "Returns a list of PlayerCountryMappingDTOs that have a player's id and their country tag."
    )]
    public async Task<IActionResult> GetCountryMappingAsync()
    {
        IEnumerable<PlayerCountryMappingDTO> mapping = await _playerService.GetCountryMappingAsync();
        return Ok(mapping);
    }
}
