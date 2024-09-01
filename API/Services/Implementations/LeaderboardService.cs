using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Enums;
using API.Services.Interfaces;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class LeaderboardService(
    IPlayersRepository playerRepository,
    IBaseStatsService baseStatsService,
    IMatchRatingStatsRepository ratingStatsRepository,
    IPlayerService playerService,
    IPlayerStatsService playerStatsService
    ) : ILeaderboardService
{
    public async Task<LeaderboardDTO> GetLeaderboardAsync(
        LeaderboardRequestQueryDTO requestQuery,
        int? authorizedPlayerId = null
    )
    {
        ValidateRequest(requestQuery);

        var leaderboard = new LeaderboardDTO
        {
            Ruleset = requestQuery.Ruleset,
            TotalPlayerCount = await baseStatsService.LeaderboardCountAsync(
                requestQuery.Ruleset,
                requestQuery.ChartType,
                requestQuery.Filter,
                authorizedPlayerId
            ),
            FilterDefaults = await baseStatsService.LeaderboardFilterDefaultsAsync(
                requestQuery.Ruleset,
                requestQuery.ChartType
            )
        };

        if (authorizedPlayerId.HasValue)
        {
            var playerId = await playerService.GetIdAsync(authorizedPlayerId.Value);

            if (playerId.HasValue)
            {
                leaderboard.PlayerChart = await GetPlayerChartAsync(
                    authorizedPlayerId.Value,
                    requestQuery.Ruleset,
                    requestQuery.ChartType
                );
            }
        }

        IEnumerable<PlayerRatingStatsDTO?> baseStats = await baseStatsService.GetLeaderboardAsync(
            requestQuery.Ruleset,
            requestQuery.Page,
            requestQuery.PageSize,
            requestQuery.ChartType,
            requestQuery.Filter,
            authorizedPlayerId
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

    private static void ValidateRequest(LeaderboardRequestQueryDTO query)
    {
        if (query.Filter.MinRank < 1 || query.Filter.MinRank > query.Filter.MaxRank)
        {
            throw new ArgumentException(
                "MinRank must be greater than 0 and less than or equal to MaxRank",
                nameof(query.Filter.MinRank)
            );
        }

        if (query.Filter.MaxRank < 1 || query.Filter.MaxRank < query.Filter.MinRank)
        {
            throw new ArgumentException(
                "MaxRank must be greater than 0 and greater than or equal to MinRank",
                nameof(query.Filter.MaxRank)
            );
        }

        if (query.Filter.MinRating < 100 || query.Filter.MinRating > query.Filter.MaxRating)
        {
            throw new ArgumentException(
                "MinRating must be at least 100 and be less than or equal to MaxRating",
                nameof(query.Filter.MinRating)
            );
        }

        if (query.Filter.MaxRating < 100 || query.Filter.MaxRating < query.Filter.MinRating)
        {
            throw new ArgumentException(
                "MaxRating must be at least 100 and greater than or equal to MinRating",
                nameof(query.Filter.MaxRating)
            );
        }

        if (query.Filter.MinMatches < 0 || query.Filter.MinMatches > query.Filter.MaxMatches)
        {
            throw new ArgumentException(
                "MinMatches must be at least 0 and less than or equal to MaxMatches",
                nameof(query.Filter.MinMatches)
            );
        }

        if (query.Filter.MaxMatches < 0 || query.Filter.MaxMatches < query.Filter.MinMatches)
        {
            throw new ArgumentException(
                "MaxMatches must be at least 1 and greater than or equal to MinMatches",
                nameof(query.Filter.MaxMatches)
            );
        }

        if (query.Filter.MinWinRate < 0 || query.Filter.MinWinRate > query.Filter.MaxWinRate)
        {
            throw new ArgumentException(
                "MinWinrate must be greater than 0 and less than or equal to MaxWinrate",
                nameof(query.Filter.MinWinRate)
            );
        }

        if (query.Filter.MaxWinRate < 0 || query.Filter.MaxWinRate < query.Filter.MinWinRate)
        {
            throw new ArgumentException(
                "MaxWinrate must be greater than 0 and greater than or equal to MinWinrate",
                nameof(query.Filter.MaxWinRate)
            );
        }

        if (query.Filter.MinWinRate > 1 || query.Filter.MaxWinRate > 1)
        {
            throw new ArgumentException("Winrate must be between 0 and 1", nameof(query.Filter.MinWinRate));
        }
    }

    private async Task<LeaderboardPlayerChartDTO?> GetPlayerChartAsync(
        int playerId,
        Ruleset ruleset,
        LeaderboardChartType chartType
    )
    {
        PlayerRatingStatsDTO? baseStats = await baseStatsService.GetAsync(null, playerId, ruleset);

        if (baseStats == null)
        {
            return null;
        }

        var rank = chartType switch
        {
            LeaderboardChartType.Global => baseStats.GlobalRank,
            LeaderboardChartType.Country => baseStats.CountryRank,
            _ => throw new ArgumentOutOfRangeException(nameof(chartType), chartType, null)
        };

        var highestRank = chartType switch
        {
            LeaderboardChartType.Global
                => await ratingStatsRepository.HighestGlobalRankAsync(playerId, ruleset),
            LeaderboardChartType.Country
                => await ratingStatsRepository.HighestCountryRankAsync(playerId, ruleset),
            _ => throw new ArgumentOutOfRangeException(nameof(chartType), chartType, null)
        };

        PlayerRankChartDTO rankChart = await playerStatsService.GetRankChartAsync(playerId, ruleset, chartType);

        return new LeaderboardPlayerChartDTO
        {
            Rank = rank,
            HighestRank = highestRank,
            Percentile = baseStats.Percentile,
            Rating = baseStats.Rating,
            Matches = baseStats.MatchesPlayed,
            WinRate = baseStats.WinRate,
            RankChart = rankChart
        };
    }
}
