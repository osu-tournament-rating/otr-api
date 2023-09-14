using API.DTOs;
using API.Entities;
using API.Services.Implementations;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : Controller
{
	private readonly ILogger<LoginController> _logger;
	private readonly IConfiguration _configuration;
	private readonly IPlayerService _playerService;
	private readonly IUserService _userService;

	public LoginController(ILogger<LoginController> logger, IConfiguration configuration, 
		IPlayerService playerService, IUserService userService)
	{
		_logger = logger;
		_configuration = configuration;
		_playerService = playerService;
		_userService = userService;
	}
	
	[AllowAnonymous]
	[HttpPost]
	public async Task<IActionResult> Login([FromBody] long osuUserId)
	{
		_logger.LogDebug("Attempting login for user with osu id {OsuId}", osuUserId);
		
		var player = await _playerService.GetPlayerDTOByOsuIdAsync(osuUserId);
		if (player == null)
		{
			await _playerService.CreateAsync(new Player
			{
				OsuId = osuUserId
			});
		}

		await AuthenticateUserAsync(osuUserId);

		var tokenString = GenerateJSONWebToken();
		return Ok(new {token = tokenString});
	}

	private string GenerateJSONWebToken()
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
			_configuration["Jwt:Issuer"],
			expires: DateTime.Now.AddMinutes(120),
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
	
	private async Task<User> AuthenticateUserAsync(long osuUserId)
	{
		var validatedPlayer = await _playerService.GetPlayerByOsuIdAsync(osuUserId);
		if (validatedPlayer == null)
		{
			// Create missing player
			validatedPlayer = await _playerService.CreateAsync(new Player
			{
				OsuId = osuUserId
			});
		}

		var user = await _userService.GetForPlayerAsync(validatedPlayer.Id);
		if (user != null)
		{
			// First time visitor
			user.LastLogin = DateTime.UtcNow;
			user.Updated = DateTime.UtcNow;
			await _userService.UpdateAsync(user);
		}
		
		return await _userService.CreateAsync(new User
		{
			PlayerId = validatedPlayer.Id,
			Created = DateTime.UtcNow,
			LastLogin = DateTime.UtcNow,
			Roles = string.Empty,
		});
	}
}