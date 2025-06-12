using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Common.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class PlayerService(
    IPlayersRepository playerRepository,
    IPlayerTournamentStatsRepository playerTournamentStatsRepository,
    IMapper mapper
) : IPlayerService
{
    public async Task<PlayerCompactDTO?> GetVersatileAsync(string key) =>
        mapper.Map<PlayerCompactDTO?>(await playerRepository.GetVersatileAsync(key, false));

    public async Task<IEnumerable<PlayerCompactDTO>> GetAsync(IEnumerable<long> osuIds)
    {
        return mapper.Map<IEnumerable<PlayerCompactDTO>>(await playerRepository.GetAsync(osuIds));
    }

    public async Task<IEnumerable<TournamentCompactDTO>?> GetTournamentsAsync(string key, Ruleset? ruleset = null, DateTime? dateMin = null, DateTime? dateMax = null)
    {
        Player? player = await playerRepository.GetVersatileAsync(key, false);
        if (player == null)
        {
            return null;
        }

        var allTournamentStats = new List<PlayerTournamentStats>();

        if (ruleset.HasValue)
        {
            // Get tournaments for specific ruleset
            ICollection<PlayerTournamentStats> stats = await playerTournamentStatsRepository.GetForPlayerAsync(
                player.Id,
                ruleset.Value,
                dateMin,
                dateMax
            );
            allTournamentStats.AddRange(stats);
        }
        else
        {
            // Get tournaments for all rulesets
            foreach (Ruleset r in Enum.GetValues<Ruleset>())
            {
                ICollection<PlayerTournamentStats> stats = await playerTournamentStatsRepository.GetForPlayerAsync(
                    player.Id,
                    r,
                    dateMin,
                    dateMax
                );
                allTournamentStats.AddRange(stats);
            }
        }

        IOrderedEnumerable<Tournament> tournaments = allTournamentStats
            .Select(pts => pts.Tournament)
            .DistinctBy(t => t.Id)
            .OrderByDescending(t => t.StartTime);

        return mapper.Map<IEnumerable<TournamentCompactDTO>>(tournaments);
    }
}
