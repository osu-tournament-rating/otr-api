using API.DTOs.Audit;
using Common.Enums;

namespace API.Services.Interfaces;

public interface IAuditService
{
    Task<IEnumerable<AuditDTO>> GetAuditsAsync(AuditEntityType entityType, int entityId);
    Task<IEnumerable<AuditDTO>> GetAuditsAsync(int userId);
}
