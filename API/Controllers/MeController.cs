using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MeController : Controller
{
	private readonly IPlayerService _playerService;

	public MeController(IPlayerService playerService) { _playerService = playerService; }

	[HttpGet]
	public async Task<ActionResult<PlayerDTO>> GetAsync([FromQuery]int offsetDays = -1)
	{
		if(!HttpContext.User.HasClaim(x => x.Type == JwtRegisteredClaimNames.Name))
		{
			return Unauthorized();
		}
		
		string? osuId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;
		if (osuId == null)
		{
			return Unauthorized();
		}
		
		if(!long.TryParse(osuId, out long res))
		{
			return BadRequest("User's login seems corrupted.");
		}
		
		
		var player = await _playerService.GetPlayerDTOByOsuIdAsync(res, true, offsetDays);
		return Ok(player);
	}
}