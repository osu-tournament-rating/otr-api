using API.DTOs.Audit;
using API.Services.Interfaces;
using AutoMapper;
using Common.Enums;
using Database;
using Database.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

/// <summary>
/// Retrieves audit logs for entities
/// </summary>
public class AuditService(OtrContext context, IMapper mapper) : IAuditService
{
    public async Task<IEnumerable<AuditDTO>> GetAuditsAsync(AuditEntityType entityType, int entityId)
    {
        List<IAuditEntity> audits = [];
        switch (entityType)
        {
            case AuditEntityType.Game:
                audits.AddRange(await context.GameAudits.Where(a => a.ReferenceIdLock == entityId).ToListAsync());
                break;
            case AuditEntityType.GameScore:
                audits.AddRange(await context.GameScoreAudits.Where(a => a.ReferenceIdLock == entityId).ToListAsync());
                break;
            case AuditEntityType.Match:
                audits.AddRange(await context.MatchAudits.Where(a => a.ReferenceIdLock == entityId).ToListAsync());
                break;
            case AuditEntityType.Tournament:
                audits.AddRange(await context.TournamentAudits.Where(a => a.ReferenceIdLock == entityId).ToListAsync());
                break;
            default:
                return new List<AuditDTO>();
        }
        return mapper.Map<IEnumerable<AuditDTO>>(audits);
    }

    public async Task<IEnumerable<AuditDTO>> GetAuditsAsync(int userId)
    {
        var gameAudits = await context.GameAudits.Where(a => a.ActionUserId == userId).ToListAsync();
        var gameScoreAudits = await context.GameScoreAudits.Where(a => a.ActionUserId == userId).ToListAsync();
        var matchAudits = await context.MatchAudits.Where(a => a.ActionUserId == userId).ToListAsync();
        var tournamentAudits = await context.TournamentAudits.Where(a => a.ActionUserId == userId).ToListAsync();

        var allAudits = new List<IAuditEntity>();
        allAudits.AddRange(gameAudits);
        allAudits.AddRange(gameScoreAudits);
        allAudits.AddRange(matchAudits);
        allAudits.AddRange(tournamentAudits);

        return mapper.Map<IEnumerable<AuditDTO>>(allAudits.OrderByDescending(a => a.Created));
    }
}
