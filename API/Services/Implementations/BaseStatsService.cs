using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using Database.Entities;
using Database.Enums;

namespace API.Services.Implementations;

public class BaseStatsService(
    IBaseStatsRepository baseStatsRepository,
    IPlayerMatchStatsRepository matchStatsRepository,
    IMatchRatingStatsRepository ratingStatsRepository,
    IApiPlayersRepository playerRepository,
    ITournamentsService tournamentsService
    ) : IBaseStatsService
{
    public async Task<IEnumerable<BaseStatsDTO?>> GetAsync(long osuPlayerId)
    {
        var id = await playerRepository.GetIdAsync(osuPlayerId);

        if (!id.HasValue)
        {
            return new List<BaseStatsDTO?>();
        }

        IEnumerable<BaseStats> baseStats = await baseStatsRepository.GetForPlayerAsync(osuPlayerId);
        var ret = new List<BaseStatsDTO?>();

        foreach (BaseStats stat in baseStats)
        {
            // One per mode
            ret.Add(await GetAsync(stat, id.Value, (int)stat.Mode));
        }

        return ret;
    }

    public async Task<BaseStatsDTO?> GetAsync(BaseStats? currentStats, int playerId, int mode)
    {
        currentStats ??= await baseStatsRepository.GetForPlayerAsync(playerId, mode);

        if (currentStats == null)
        {
            return null;
        }

        var matchesPlayed = await matchStatsRepository.CountMatchesPlayedAsync(playerId, mode);
        var winRate = await matchStatsRepository.GlobalWinrateAsync(playerId, mode);
        var highestGlobalRank = await ratingStatsRepository.HighestGlobalRankAsync(playerId, mode);
        var tournamentsPlayed = await tournamentsService.CountPlayedAsync(playerId, mode);
        var rankProgress = new RankProgressDTO
        {
            CurrentTier = RatingUtils.GetTier(currentStats.Rating),
            CurrentSubTier = RatingUtils.GetSubTier(currentStats.Rating),
            RatingForNextTier = RatingUtils.GetNextTierRatingDelta(currentStats.Rating),
            RatingForNextMajorTier = RatingUtils.GetNextMajorTierRatingDelta(currentStats.Rating),
            NextMajorTier = RatingUtils.GetNextMajorTier(currentStats.Rating),
            SubTierFillPercentage = RatingUtils.GetNextTierFillPercentage(currentStats.Rating),
            MajorTierFillPercentage = RatingUtils.GetNextMajorTierFillPercentage(currentStats.Rating)
        };

        return new BaseStatsDTO
        {
            PlayerId = playerId,
            AverageMatchCost = currentStats.MatchCostAverage,
            CountryRank = currentStats.CountryRank,
            GlobalRank = currentStats.GlobalRank,
            MatchesPlayed = matchesPlayed,
            Mode = mode,
            Percentile = currentStats.Percentile,
            Rating = currentStats.Rating,
            Volatility = currentStats.Volatility,
            WinRate = winRate,
            HighestGlobalRank = highestGlobalRank,
            TournamentsPlayed = tournamentsPlayed,
            RankProgress = rankProgress
        };
    }

    public async Task<int> BatchInsertAsync(IEnumerable<BaseStatsPostDTO> stats)
    {
        var toInsert = new List<BaseStats>();
        foreach (BaseStatsPostDTO item in stats)
        {
            toInsert.Add(
                new BaseStats
                {
                    PlayerId = item.PlayerId,
                    MatchCostAverage = item.MatchCostAverage,
                    Rating = item.Rating,
                    Volatility = item.Volatility,
                    Mode = (Ruleset)item.Mode,
                    Percentile = item.Percentile,
                    GlobalRank = item.GlobalRank,
                    CountryRank = item.CountryRank
                }
            );
        }

        return await baseStatsRepository.BatchInsertAsync(toInsert);
    }

    public async Task<IEnumerable<BaseStatsDTO?>> GetLeaderboardAsync(
        int mode,
        int page,
        int pageSize,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO filter,
        int? playerId
    )
    {
        IEnumerable<BaseStats> baseStats = await baseStatsRepository.GetLeaderboardAsync(
            page,
            pageSize,
            mode,
            chartType,
            filter,
            playerId
        );

        var leaderboard = new List<BaseStatsDTO?>();

        foreach (BaseStats baseStat in baseStats)
        {
            leaderboard.Add(await GetAsync(baseStat, baseStat.PlayerId, mode));
        }

        return leaderboard;
    }

    public async Task TruncateAsync() => await baseStatsRepository.TruncateAsync();

    public async Task<int> LeaderboardCountAsync(
        int requestQueryMode,
        LeaderboardChartType requestQueryChartType,
        LeaderboardFilterDTO requestQueryFilter,
        int? playerId
    ) =>
        await baseStatsRepository.LeaderboardCountAsync(
            requestQueryMode,
            requestQueryChartType,
            requestQueryFilter,
            playerId
        );

    public async Task<LeaderboardFilterDefaultsDTO> LeaderboardFilterDefaultsAsync(
        int requestQueryMode,
        LeaderboardChartType requestQueryChartType
    ) =>
        new()
        {
            MaxRating = await baseStatsRepository.HighestRatingAsync(requestQueryMode),
            MaxMatches = await baseStatsRepository.HighestMatchesAsync(requestQueryMode),
            MaxRank = 100_000
        };

    public async Task<IDictionary<int, int>> GetHistogramAsync(int mode) =>
        await baseStatsRepository.GetHistogramAsync(mode);
}
