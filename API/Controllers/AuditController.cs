using API.Authorization;
using API.DTOs.Audit;
using API.Services.Interfaces;
using Asp.Versioning;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuditController(IAuditService auditService) : Controller
{
    /// <summary>
    /// Get audits for a specific entity
    /// </summary>
    /// <param name="entityType">The type of entity to get audits for</param>
    /// <param name="entityId">The ID of the entity to get audits for</param>
    /// <response code="200">Returns a list of audits for the specified entity</response>
    [HttpGet("entity/{entityType}/{entityId:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType<IEnumerable<AuditDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEntityAuditsAsync(AuditEntityType entityType, int entityId)
    {
        IEnumerable<AuditDTO> audits = await auditService.GetAuditsAsync(entityType, entityId);
        return Ok(audits);
    }

    /// <summary>
    /// Get audits performed by a specific user
    /// </summary>
    /// <param name="userId">The ID of the user to get audits for</param>
    /// <response code="200">Returns a list of audits performed by the specified user</response>
    [HttpGet("user/{userId:int}")]
    [Authorize(Roles = OtrClaims.Roles.Admin)]
    [ProducesResponseType<IEnumerable<AuditDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserAuditsAsync(int userId)
    {
        IEnumerable<AuditDTO> audits = await auditService.GetAuditsAsync(userId);
        return Ok(audits);
    }
}
