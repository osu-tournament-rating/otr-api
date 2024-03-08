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
[Authorize(Roles = "user")]
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

    [HttpGet("{osuId:long}/info")]
    public async Task<ActionResult<PlayerInfoDTO?>> GetByUserIdAsync(long osuId)
    {
        PlayerInfoDTO? info = await _playerService.GetAsync(osuId);

        if (info == null)
        {
            return NotFound($"User with osuid {osuId} does not exist");
        }

        return info;
    }

    [HttpGet("{username}/info")]
    public async Task<ActionResult<PlayerInfoDTO?>> GetByUserIdAsync(string username)
    {
        PlayerInfoDTO? player = await _playerService.GetAsync(username);
        if (player != null)
        {
            return Ok(player);
        }

        return NotFound($"User with username {username} does not exist");
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
