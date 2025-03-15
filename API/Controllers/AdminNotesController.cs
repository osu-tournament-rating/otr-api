using API.Utilities.AdminNotes;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[AllowAnonymous]
[ValidateAdminNoteControllerRoute]
[Route("api/v{version:apiVersion}/notes/{entity}")]
public class AdminNotesController : ControllerBase
{
    [FromRoute(Name = "entity")]
    public AdminNoteRouteTarget AdminNoteTarget { get; } = null!;

    [HttpGet]
    public IActionResult GetNotes()
    {
        return Ok();
    }
}
