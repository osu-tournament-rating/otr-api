using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Services.Interfaces;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class LeaderboardService(
    IPlayersRepository playerRepository,
    IBaseStatsService baseStatsService
    ) : ILeaderboardService
{
    public async Task<LeaderboardDTO> GetLeaderboardAsync(
        LeaderboardRequestQueryDTO requestQuery,
        int? authorizedUserId = null
    )
    {
        var leaderboard = new LeaderboardDTO
        {
            Ruleset = requestQuery.Ruleset,
            TotalPlayerCount = await baseStatsService.LeaderboardCountAsync(
                requestQuery.Ruleset,
                requestQuery.ChartType,
                requestQuery.Filter,
                authorizedUserId
            ),
            FilterDefaults = await baseStatsService.LeaderboardFilterDefaultsAsync(
                requestQuery.Ruleset,
                requestQuery.ChartType
            )
        };

        IEnumerable<PlayerRatingStatsDTO?> baseStats = await baseStatsService.GetLeaderboardAsync(
            requestQuery.Ruleset,
            requestQuery.Page,
            requestQuery.PageSize,
            requestQuery.ChartType,
            requestQuery.Filter,
            authorizedUserId
        );

        var leaderboardPlayerInfo = new List<LeaderboardPlayerInfoDTO>();

        foreach (PlayerRatingStatsDTO? baseStat in baseStats)
        {
            if (baseStat == null)
            {
                continue;
            }

            var osuId = await playerRepository.GetOsuIdAsync(baseStat.PlayerId);
            var name = await playerRepository.GetUsernameAsync(baseStat.PlayerId);
            var country = await playerRepository.GetCountryAsync(baseStat.PlayerId);

            leaderboardPlayerInfo.Add(
                new LeaderboardPlayerInfoDTO
                {
                    PlayerId = baseStat.PlayerId,
                    OsuId = osuId,
                    GlobalRank = baseStat.GlobalRank,
                    MatchesPlayed = baseStat.MatchesPlayed,
                    Name = name ?? "<Unknown>",
                    Rating = baseStat.Rating,
                    Tier = baseStat.RankProgress.CurrentTier,
                    WinRate = baseStat.WinRate,
                    Ruleset = baseStat.Ruleset,
                    Country = country
                }
            );
        }

        leaderboard.Leaderboard = leaderboardPlayerInfo.OrderBy(x => x.GlobalRank);
        return leaderboard;
    }
}
