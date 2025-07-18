﻿using API.DTOs;
using API.Services.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/stats")]
public class PlatformStatsController(IPlatformStatsService statsService) : Controller
{
    private const int StatsCacheDurationSeconds = 10;

    /// <summary>
    /// Get various platform-wide stats
    /// </summary>
    /// <response code="200">Returns various platform-wide stats</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<PlatformStatsDTO>(StatusCodes.Status200OK)]
    [OutputCache(Duration = StatsCacheDurationSeconds)]
    public async Task<IActionResult> GetAsync() => Ok(await statsService.GetAsync());
}
