using API.DTOs;
using API.Entities;
using API.Enums;
using API.Osu;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;

namespace API.Services.Implementations;

public class BaseStatsService(
    IBaseStatsRepository baseStatsRepository,
    IPlayerMatchStatsRepository matchStatsRepository,
    IMatchRatingStatsRepository ratingStatsRepository,
    IPlayerRepository playerRepository,
    ITournamentsService tournamentsService
    ) : IBaseStatsService
{
    private readonly IBaseStatsRepository _baseStatsRepository = baseStatsRepository;
    private readonly IPlayerMatchStatsRepository _matchStatsRepository = matchStatsRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly ITournamentsService _tournamentsService = tournamentsService;
    private readonly IMatchRatingStatsRepository _ratingStatsRepository = ratingStatsRepository;

    public async Task<IEnumerable<BaseStatsDTO?>> GetAsync(long osuPlayerId)
    {
        var id = await _playerRepository.GetIdAsync(osuPlayerId);

        if (!id.HasValue)
        {
            return new List<BaseStatsDTO?>();
        }

        IEnumerable<BaseStats> baseStats = await _baseStatsRepository.GetForPlayerAsync(osuPlayerId);
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
        currentStats ??= await _baseStatsRepository.GetForPlayerAsync(playerId, mode);

        if (currentStats == null)
        {
            return null;
        }

        var matchesPlayed = await _matchStatsRepository.CountMatchesPlayedAsync(playerId, mode);
        var winRate = await _matchStatsRepository.GlobalWinrateAsync(playerId, mode);
        var highestGlobalRank = await _ratingStatsRepository.HighestGlobalRankAsync(playerId, mode);
        var tournamentsPlayed = await _tournamentsService.CountPlayedAsync(playerId, mode);
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
            Winrate = winRate,
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
                    Mode = (OsuEnums.Ruleset)item.Mode,
                    Percentile = item.Percentile,
                    GlobalRank = item.GlobalRank,
                    CountryRank = item.CountryRank
                }
            );
        }

        return await _baseStatsRepository.BatchInsertAsync(toInsert);
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
        IEnumerable<BaseStats> baseStats = await _baseStatsRepository.GetLeaderboardAsync(
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

    public async Task TruncateAsync() => await _baseStatsRepository.TruncateAsync();

    public async Task<int> LeaderboardCountAsync(
        int requestQueryMode,
        LeaderboardChartType requestQueryChartType,
        LeaderboardFilterDTO requestQueryFilter,
        int? playerId
    ) =>
        await _baseStatsRepository.LeaderboardCountAsync(
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
            MaxRating = await _baseStatsRepository.HighestRatingAsync(requestQueryMode),
            MaxMatches = await _baseStatsRepository.HighestMatchesAsync(requestQueryMode),
            MaxRank = 100_000
        };

    public async Task<IDictionary<int, int>> GetHistogramAsync(int mode) =>
        await _baseStatsRepository.GetHistogramAsync(mode);
}
