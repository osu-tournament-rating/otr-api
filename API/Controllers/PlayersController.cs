using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Authorize(Roles = "user, client")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PlayersController(IPlayerService playerService) : Controller
{
    private readonly IPlayerService _playerService = playerService;

    /// <summary>
    /// List all players
    /// </summary>
    /// <response code="200">Returns all players</response>
    [HttpGet]
    [Authorize(Roles = "system")]
    [ProducesResponseType<IEnumerable<PlayerDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync()
    {
        IEnumerable<PlayerDTO> players = await _playerService.GetAllAsync();
        return Ok(players);
    }

    /// <summary>
    /// Get player info by versatile search
    /// </summary>
    /// <remarks>Get player info searching first by id, then osuId, then username</remarks>
    /// <param name="key"></param>
    /// <response code="404">If a player with matching key does not exist</response>
    /// <response code="200">Returns player info</response>
    [HttpGet("{key}/info")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<PlayerDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInfoAsync(string key)
    {
        PlayerDTO? result = await _playerService.GetVersatileAsync(key);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// List all player ranks
    /// </summary>
    /// <response code="200">Returns all player ranks</response>
    [HttpGet("ranks")]
    [Authorize(Roles = "system, admin")]
    [ProducesResponseType<IEnumerable<PlayerRanksDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRanksAsync()
    {
        IEnumerable<PlayerRanksDTO> result = await _playerService.GetAllRanksAsync();
        return Ok(result);
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
