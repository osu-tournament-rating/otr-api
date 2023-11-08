using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OsuSharp.Exceptions;
using OsuSharp.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers;

public class LoginWrapper
{
	public string Code { get; set; }
}

[ApiController]
[EnableCors]
[Route("api/[controller]")]
public class LoginController : Controller
{
	private readonly IConfiguration _configuration;
	private readonly ILogger<LoginController> _logger;
	private readonly IOsuClient _osuClient;
	private readonly IPlayerRepository _playerRepository;
	private readonly IUserRepository _userRepository;

	public LoginController(ILogger<LoginController> logger, IConfiguration configuration,
		IPlayerRepository playerRepository, IUserRepository userRepository, IOsuClient osuClient)
	{
		_logger = logger;
		_configuration = configuration;
		_playerRepository = playerRepository;
		_userRepository = userRepository;
		_osuClient = osuClient;
	}

	[AllowAnonymous]
	[HttpPost]
	public async Task<IActionResult> Login([FromQuery] string code)
	{
		_logger.LogDebug("Attempting login for user with code {Code}", code);

		if (string.IsNullOrEmpty(code))
		{
			return BadRequest("Missing code");
		}

		try
		{
			var osuUser = await AuthorizeAsync(code);
			long osuUserId = osuUser.Id;

			var player = await _playerRepository.GetPlayerByOsuIdAsync(osuUserId);
			if (player == null)
			{
				player = await _playerRepository.CreateAsync(new Player
				{
					OsuId = osuUserId
				});
			}

			if (player == null)
			{
				throw new Exception("Critical error, failed to fetch player from database when logging in");
			}

			var user = await AuthenticateUserAsync(player);

			if (user == null)
			{
				throw new Exception("Critical error, failed to fetch user from database");
			}

			// Safe to force not-null on PlayerId as we are not attempting a system login
			string tokenString = GenerateJSONWebToken(user, user.PlayerId.ToString()!);

			bool secure = false;
#if !DEBUG
			secure = true;
#endif
			Response.Cookies.Append("OTR-Access-Token", tokenString, new CookieOptions
				{ HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = secure, IsEssential = true});

			return Ok();
		}
		catch (ApiException e)
		{
			_logger.LogWarning(e, "Too many requests, aborting processing of login request");
			return StatusCode(429, "Too many requests! This endpoint may be under stress.");
		}
	}

	[AllowAnonymous]
	[HttpPost("system")]
	public async Task<IActionResult> SystemLogin()
	{
		_logger.LogInformation("Attempting system login");

		if (!HttpContext.Request.Headers.ContainsKey("Authorization"))
		{
			return Unauthorized("Missing authorization header");
		}
		
		_logger.LogDebug("Authorization header found, validating");
		string validationKey = _configuration["Auth:PrivilegedClientSecret"] ?? throw new Exception("Missing Auth:PrivilegedClientSecret in configuration!!");
		if (HttpContext.Request.Headers["Authorization"] != validationKey)
		{
			return Unauthorized("Invalid authorization header");
		}

		var user = await AuthenticateSystemUserAsync();

		if (user == null)
		{
			return StatusCode(500, "Failed to fetch system user from database");
		}

		string tokenString = GenerateJSONWebToken(user, "system");
		return Ok(new { token = tokenString });
	}

	private string GenerateJSONWebToken(User? user, string name)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		if (user == null)
		{
			throw new ArgumentNullException(nameof(user));
		}

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Name, name)
		};

		foreach (string role in user.Roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role));
		}

		var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
			_configuration["Jwt:Issuer"],
			claims,
			expires: DateTime.Now.AddDays(7),
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	private async Task<User?> AuthenticateUserAsync(Player associatedPlayer)
	{
		var user = await _userRepository.GetForPlayerAsync(associatedPlayer.Id);
		if (user == null)
		{
			// First time visitor
			return await _userRepository.CreateAsync(new User
			{
				PlayerId = associatedPlayer.Id,
				Created = DateTime.UtcNow,
				LastLogin = DateTime.UtcNow,
				Roles = Array.Empty<string>()
			});
		}

		user.LastLogin = DateTime.UtcNow;
		user.Updated = DateTime.UtcNow;
		user.PlayerId = associatedPlayer.Id;
		await _userRepository.UpdateAsync(user);
		return user;
	}

	private async Task<IGlobalUser> AuthorizeAsync(string osuCode)
	{
		string cbUrl = _configuration["Auth:ClientCallbackUrl"] ?? throw new Exception("Missing Auth:ClientCallbackUrl in configuration!!");
		// Use OsuSharp to validate that the user is who they say they are
		await _osuClient.GetAccessTokenFromCodeAsync(osuCode, cbUrl);
		return await _osuClient.GetCurrentUserAsync();
	}

	private async Task<User?> AuthenticateSystemUserAsync()
	{
		var user = await _userRepository.GetOrCreateSystemUserAsync();

		if (user == null)
		{
			return null;
		}
		
		user.LastLogin = DateTime.UtcNow;
		user.Updated = DateTime.UtcNow;
		await _userRepository.UpdateAsync(user);
		_logger.LogInformation("Authenticated system user");
		return user;
	}
}