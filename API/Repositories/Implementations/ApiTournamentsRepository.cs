using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using Database;
using Database.Enums;
using Database.Enums.Verification;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class ApiTournamentsRepository(OtrContext context, IBeatmapsRepository beatmapsRepository) :
    TournamentsRepository(context, beatmapsRepository), IApiTournamentsRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<TournamentSearchResultDTO>> SearchAsync(string name)
    {
        //_ is a wildcard character in psql so it needs to have an escape character added in front of it.
        name = name.Replace("_", @"\_");
        return await _context.Tournaments
            .AsNoTracking()
            .Where(x =>
                EF.Functions.ILike(x.Name, $"%{name}%", @"\")
                || EF.Functions.ILike(x.Abbreviation, $"%{name}%", @"\")
            )
            .Select(t => new TournamentSearchResultDTO()
            {
                Id = t.Id,
                Ruleset = t.Ruleset,
                LobbySize = t.LobbySize,
                Name = t.Name
            })
            .Take(30)
            .ToListAsync();
    }

    public async Task<PlayerTournamentLobbySizeCountDTO> GetLobbySizeStatsAsync(
    int playerId,
    Ruleset ruleset,
    DateTime dateMin,
    DateTime dateMax
)
    {
        var participatedTournaments =
            await QueryForParticipation(playerId, ruleset, dateMin, dateMax)
            .Select(t => new { TournamentId = t.Id, TeamSize = t.LobbySize })
            .Distinct() // Ensures each tournament is counted once
            .ToListAsync();

        return new PlayerTournamentLobbySizeCountDTO
        {
            Count1v1 = participatedTournaments.Count(x => x.TeamSize == 1),
            Count2v2 = participatedTournaments.Count(x => x.TeamSize == 2),
            Count3v3 = participatedTournaments.Count(x => x.TeamSize == 3),
            Count4v4 = participatedTournaments.Count(x => x.TeamSize == 4),
            CountOther = participatedTournaments.Count(x => x.TeamSize > 4)
        };
    }

    public async Task<IEnumerable<PlayerTournamentMatchCostDTO>> GetPerformancesAsync(int playerId,
        Ruleset ruleset,
        DateTime dateMin,
        DateTime dateMax,
        int count,
        TournamentPerformanceResultType performanceType)
    {
        IQueryable<PlayerTournamentMatchCostDTO> query =
            QueryForParticipation(playerId, ruleset, dateMin, dateMax)
            .Select(t => new PlayerTournamentMatchCostDTO
            {
                PlayerId = playerId,
                Ruleset = ruleset,
                TournamentId = t.Id,
                TournamentName = t.Name,
                TournamentAcronym = t.Abbreviation,
                MatchCost = t.Matches
                    .Where(m => m.VerificationStatus == VerificationStatus.Verified)
                    .SelectMany(m => m.PlayerMatchStats)
                    .Where(pms => pms.PlayerId == playerId)
                    .Average(pms => pms.MatchCost)
            });

        query = performanceType switch
        {
            TournamentPerformanceResultType.Best => query.OrderByDescending(d => d.MatchCost),
            TournamentPerformanceResultType.Worst => query.OrderBy(d => d.MatchCost),
            TournamentPerformanceResultType.Recent => query,
            _ => throw new ArgumentOutOfRangeException(nameof(performanceType), performanceType, null)
        };

        return await query.Take(count).ToListAsync();
    }
}
