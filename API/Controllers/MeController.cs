using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[EnableCors]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "user")]
[Authorize(Roles = "whitelist")]
public class MeController(IUserService userService, IPlayerStatsService playerStatsService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly IPlayerStatsService _playerStatsService = playerStatsService;

    [HttpGet]
    [EndpointSummary("Get the currently logged in user")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, Ok<UserInfoDTO>>> GetAsync()
    {
        var id = HttpContext.AuthorizedUserIdentity();
        if (!id.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        UserInfoDTO? user = await _userService.GetAsync(id.Value);
        if (user?.OsuId == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(user);
    }

    [HttpGet("stats")]
    [EndpointSummary("Get stats for the currently logged in user")]
    public async Task<Results<UnauthorizedHttpResult, NotFound, Ok<PlayerStatsDTO>>> GetStatsAsync(
        [FromQuery] int mode = 0,
        [FromQuery] DateTime? dateMin = null,
        [FromQuery] DateTime? dateMax = null
    )
    {
        var userId = HttpContext.AuthorizedUserIdentity();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var playerId = (await _userService.GetAsync(userId.Value))?.PlayerId;
        if (!playerId.HasValue)
        {
            return TypedResults.NotFound();
        }

        PlayerStatsDTO result = await _playerStatsService.GetAsync(
            playerId.Value,
            null,
            mode,
            dateMin ?? DateTime.MinValue,
            dateMax ?? DateTime.UtcNow
        );
        return TypedResults.Ok(result);
    }
}
