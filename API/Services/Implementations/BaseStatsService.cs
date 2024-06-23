using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using Database.Entities;
using Database.Entities.Processor;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class BaseStatsService(
    IApiBaseStatsRepository baseStatsRepository,
    IPlayerMatchStatsRepository matchStatsRepository,
    IMatchRatingStatsRepository ratingStatsRepository,
    IApiPlayersRepository playerRepository,
    ITournamentsService tournamentsService
    ) : IBaseStatsService
{
    public async Task<IEnumerable<PlayerRatingDTO?>> GetAsync(long osuPlayerId)
    {
        var id = await playerRepository.GetIdAsync(osuPlayerId);

        if (!id.HasValue)
        {
            return new List<PlayerRatingDTO?>();
        }

        IEnumerable<PlayerRating> baseStats = await baseStatsRepository.GetForPlayerAsync(osuPlayerId);
        var ret = new List<PlayerRatingDTO?>();

        foreach (PlayerRating stat in baseStats)
        {
            // One per mode
            ret.Add(await GetAsync(stat, id.Value, (int)stat.Ruleset));
        }

        return ret;
    }

    public async Task<PlayerRatingDTO?> GetAsync(PlayerRating? currentStats, int playerId, int mode)
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

        return new PlayerRatingDTO
        {
            PlayerId = playerId,
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
        var toInsert = new List<PlayerRating>();
        foreach (BaseStatsPostDTO item in stats)
        {
            toInsert.Add(
                new PlayerRating
                {
                    PlayerId = item.PlayerId,
                    Rating = item.Rating,
                    Volatility = item.Volatility,
                    Ruleset = (Ruleset)item.Mode,
                    Percentile = item.Percentile,
                    GlobalRank = item.GlobalRank,
                    CountryRank = item.CountryRank
                }
            );
        }

        return await baseStatsRepository.BatchInsertAsync(toInsert);
    }

    public async Task<IEnumerable<PlayerRatingDTO?>> GetLeaderboardAsync(
        int mode,
        int page,
        int pageSize,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO filter,
        int? playerId
    )
    {
        IEnumerable<PlayerRating> baseStats = await baseStatsRepository.GetLeaderboardAsync(
            page,
            pageSize,
            mode,
            chartType,
            filter,
            playerId
        );

        var leaderboard = new List<PlayerRatingDTO?>();

        foreach (PlayerRating baseStat in baseStats)
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
