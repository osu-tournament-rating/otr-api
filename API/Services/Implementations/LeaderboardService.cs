using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Services.Interfaces;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class LeaderboardService(
    IPlayersRepository playerRepository,
    IPlayerRatingsService playerRatingsService
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
            TotalPlayerCount = await playerRatingsService.LeaderboardCountAsync(
                requestQuery.Ruleset,
                requestQuery.ChartType,
                requestQuery.Filter,
                authorizedUserId
            ),
            FilterDefaults = await playerRatingsService.LeaderboardFilterDefaultsAsync(
                requestQuery.Ruleset,
                requestQuery.ChartType
            )
        };

        IEnumerable<PlayerRatingStatsDTO?> leaderboardStats = await playerRatingsService.GetLeaderboardAsync(
            requestQuery.Ruleset,
            requestQuery.Page,
            requestQuery.PageSize,
            requestQuery.ChartType,
            requestQuery.Filter,
            authorizedUserId
        );

        var leaderboardPlayerInfo = new List<LeaderboardPlayerInfoDTO>();

        foreach (PlayerRatingStatsDTO? leaderboardStat in leaderboardStats)
        {
            if (leaderboardStat == null)
            {
                continue;
            }

            var osuId = await playerRepository.GetOsuIdAsync(leaderboardStat.PlayerId);
            var name = await playerRepository.GetUsernameAsync(leaderboardStat.PlayerId);
            var country = await playerRepository.GetCountryAsync(leaderboardStat.PlayerId);

            leaderboardPlayerInfo.Add(
                new LeaderboardPlayerInfoDTO
                {
                    PlayerId = leaderboardStat.PlayerId,
                    OsuId = osuId,
                    GlobalRank = leaderboardStat.GlobalRank,
                    MatchesPlayed = leaderboardStat.MatchesPlayed,
                    Name = name ?? "<Unknown>",
                    Rating = leaderboardStat.Rating,
                    Tier = leaderboardStat.RankProgress.CurrentTier,
                    WinRate = leaderboardStat.WinRate,
                    Ruleset = leaderboardStat.Ruleset,
                    Country = country
                }
            );
        }

        leaderboard.Leaderboard = leaderboardPlayerInfo.OrderBy(x => x.GlobalRank);
        return leaderboard;
    }
}
