using API.DTOs;
using API.Osu;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[EnableCors]
[Authorize(Roles = "user")]
[Route("api/[controller]")]
public class PlayersController : Controller
{
	private readonly IPlayerService _playerService;
	public PlayersController(IPlayerService playerService) { _playerService = playerService; }

	[HttpGet("all")]
	[Authorize(Roles = "system")]
	public async Task<IActionResult> GetAllAsync()
	{
		var players = await _playerService.GetAllAsync();
		return Ok(players);
	}

	[HttpGet("{userId:int}/info")]
	public async Task<ActionResult<PlayerInfoDTO?>> GetByUserIdAsync(int userId)
	{
		var player = await _playerService.GetAsync(userId);
		if (player != null)
		{
			return Ok(player);
		}

		return NotFound($"User with id {userId} does not exist");
	}

	[HttpGet("{username}/info")]
	public async Task<ActionResult<PlayerInfoDTO?>> GetByUserIdAsync(string username)
	{
		var player = await _playerService.GetAsync(username);
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
		var ranks = await _playerService.GetAllRanksAsync();
		return Ok(ranks);
	}

	[HttpGet("id-mapping")]
	[Authorize(Roles = "system")]
	public async Task<ActionResult<IEnumerable<PlayerIdMappingDTO>>> GetIdMappingAsync()
	{
		var mapping = await _playerService.GetIdMappingAsync();
		return Ok(mapping);
	}

	[HttpGet("country-mapping")]
	[Authorize(Roles = "system")]
	[ProducesResponseType<IEnumerable<PlayerCountryMappingDTO>>(StatusCodes.Status200OK)]
	[EndpointSummary("Returns a list of PlayerCountryMappingDTOs that have a player's id and their country tag.")]
	public async Task<IActionResult> GetCountryMappingAsync()
	{
		var mapping = await _playerService.GetCountryMappingAsync();
		return Ok(mapping);
	}
}