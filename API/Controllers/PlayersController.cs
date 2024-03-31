using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Authorize(Roles = "user, client")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PlayersController(IPlayerService playerService, IMatchesService matchesService) : Controller
{
    private readonly IPlayerService _playerService = playerService;
    private readonly IMatchesService _matchesService = matchesService;

    /// <summary>
    /// List all players
    /// </summary>
    /// <response code="200">Returns all players</response>
    [HttpGet]
    [Authorize(Roles = "system, admin")]
    [ProducesResponseType<IEnumerable<PlayerDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync()
    {
        IEnumerable<PlayerDTO> players = await _playerService.GetAllAsync();
        return Ok(players);
    }

    /// <summary>
    /// Get a player by versatile search
    /// </summary>
    /// <remarks>Get a player searching first by id, then osuId, then username</remarks>
    /// <param name="key">The dynamic key of the player to look for</param>
    /// <response code="404">If a player with matching key does not exist</response>
    /// <response code="200">Returns a player</response>
    [HttpGet("{key}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<PlayerDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(string key)
    {
        PlayerDTO? result = await _playerService.GetVersatileAsync(key);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get a player's matches
    /// </summary>
    /// <remarks>Get a player's matches searching first by id, then osuId, then username</remarks>
    /// <param name="key">The dynamic key of the player to look for</param>
    /// <param name="mode">osu! ruleset</param>
    /// <param name="dateMin">Filter from earliest date. If null, earliest possible date</param>
    /// <param name="dateMax">Filter to latest date. If null, latest possible date</param>
    /// <response code="404">If a player with matching key does not exist</response>
    /// <response code="200">Returns a player's matches</response>
    [HttpGet("{key}/matches")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<MatchDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatchesAsync(
        string key,
        [FromQuery] int mode = 0,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    )
    {
        var osuId = (await _playerService.GetVersatileAsync(key))?.OsuId;

        if (!osuId.HasValue)
        {
            return NotFound();
        }

        IEnumerable<MatchDTO> matches = await _matchesService.GetAllForPlayerAsync(osuId.Value, mode,
            dateMin ?? DateTime.MinValue, dateMax ?? DateTime.MaxValue);

        return Ok(matches);
    }

    /// <summary>
    /// Update a player
    /// </summary>
    /// <param name="id">Player id</param>
    /// <param name="patch">JsonPatch data</param>
    /// <response code="404">If a player matching the given id does not exist</response>
    /// <response code="400">If JsonPatch data is malformed or there was an error updating player data</response>
    /// <response code="200">Returns the updated player</response>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "system, admin")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ModelStateDictionary>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<PlayerDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] JsonPatchDocument<PlayerDTO> patch)
    {
        PlayerDTO? target = await _playerService.GetAsync(id);

        if (target is null)
        {
            return NotFound();
        }

        if (patch.Operations.Any(op => op.OperationType != OperationType.Replace))
        {
            return BadRequest();
        }

        patch.ApplyTo(target, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        PlayerDTO? result = await _playerService.UpdateAsync(id, target);
        return result == null
            ? BadRequest()
            : Ok(result);
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
}
