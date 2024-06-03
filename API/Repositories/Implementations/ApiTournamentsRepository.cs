using API.DTOs;
using API.Repositories.Interfaces;
using Database;
using Database.Enums;
using Database.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class ApiTournamentsRepository(OtrContext context) : TournamentsRepository(context), IApiTournamentsRepository
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
                Ruleset = (Ruleset)t.Mode,
                TeamSize = t.TeamSize,
                Name = t.Name
            })
            .Take(30)
            .ToListAsync();
    }

        public async Task<PlayerTournamentTeamSizeCountDTO> GetTeamSizeStatsAsync(
        int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax
    )
    {
        var participatedTournaments =
            await QueryForParticipation(playerId, mode, dateMin, dateMax)
            .Select(t => new { TournamentId = t.Id, t.TeamSize })
            .Distinct() // Ensures each tournament is counted once
            .ToListAsync();

        return new PlayerTournamentTeamSizeCountDTO
        {
            Count1v1 = participatedTournaments.Count(x => x.TeamSize == 1),
            Count2v2 = participatedTournaments.Count(x => x.TeamSize == 2),
            Count3v3 = participatedTournaments.Count(x => x.TeamSize == 3),
            Count4v4 = participatedTournaments.Count(x => x.TeamSize == 4),
            CountOther = participatedTournaments.Count(x => x.TeamSize > 4)
        };
    }

    public async Task<IEnumerable<PlayerTournamentMatchCostDTO>> GetPerformancesAsync(int playerId,
        int mode,
        DateTime dateMin,
        DateTime dateMax,
        int count,
        bool bestPerformances)
    {
        IQueryable<PlayerTournamentMatchCostDTO> query =
            QueryForParticipation(playerId, mode, dateMin, dateMax)
            .Select(t => new PlayerTournamentMatchCostDTO()
            {
                PlayerId = playerId,
                Mode = mode,
                TournamentId = t.Id,
                TournamentName = t.Name,
                TournamentAcronym = t.Abbreviation,
                // Calc average match cost
                MatchCost = t.Matches
                    // Filter invalid matches (Above filter uses Any, so invalid matches can still be included)
                    .Where(m => m.VerificationStatus == Old_MatchVerificationStatus.Verified)
                    // Filter for ratings belonging to target player
                    .SelectMany(m => m.RatingStats)
                    .Where(mrs => mrs.PlayerId == playerId)
                    .Average(mrs => mrs.MatchCost)
            });

        // Sort
        query = bestPerformances
            ? query.OrderByDescending(d => d.MatchCost)
            : query.OrderBy(d => d.MatchCost);

        return await query.Take(count).ToListAsync();
    }
}
