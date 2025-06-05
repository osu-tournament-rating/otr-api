using API.DTOs.Audit;
using Common.Enums;

namespace API.Services.Interfaces;

/// <summary>
/// Service for retrieving audit records
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Gets audit records for a specific entity
    /// </summary>
    /// <param name="entityType">Type of entity to retrieve audits for</param>
    /// <param name="entityId">ID of the entity to retrieve audits for</param>
    /// <returns>Collection of audit records for the specified entity</returns>
    Task<IEnumerable<AuditDTO>> GetAuditsAsync(AuditEntityType entityType, int entityId);

    /// <summary>
    /// Gets all audit records performed by a specific user
    /// </summary>
    /// <param name="userId">ID of the user to retrieve audits for</param>
    /// <returns>Collection of audit records performed by the specified user, ordered by creation date descending</returns>
    Task<IEnumerable<AuditDTO>> GetAuditsAsync(int userId);
}
