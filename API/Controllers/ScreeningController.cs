using API.DTOs;
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
public class ScreeningController : Controller
{
    [HttpPost]
    [Authorize(Roles = "user, client")]
    [EndpointSummary("Allows authorized users & clients to submit a collection of " +
                     "users to be screened based on a set of criteria")]
    public async Task<Results<BadRequest, Ok>> ScreenAsync([FromBody] ScreeningDTO screening)
    {

    }
}
