using API.DTOs;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using Database.Entities.Processor;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class PlayerRatingsService(
    IApiPlayerRatingsRepository playerRatingsRepository,
    IPlayerMatchStatsRepository matchStatsRepository,
    IPlayersRepository playerRepository,
    ITournamentsService tournamentsService
) : IPlayerRatingsService
{
    public async Task<IEnumerable<PlayerRatingStatsDTO?>> GetAsync(long osuPlayerId)
    {
        var id = await playerRepository.GetIdAsync(osuPlayerId);

        if (!id.HasValue)
        {
            return [];
        }

        IEnumerable<PlayerRating> playerRatings = await playerRatingsRepository.GetAsync(osuPlayerId);
        var ret = new List<PlayerRatingStatsDTO?>();

        foreach (PlayerRating stat in playerRatings)
        {
            // One per ruleset
            ret.Add(await GetAsync(stat, id.Value, stat.Ruleset));
        }

        return ret;
    }

    public async Task<PlayerRatingStatsDTO?> GetAsync(PlayerRating? currentStats, int playerId, Ruleset ruleset)
    {
        currentStats ??= await playerRatingsRepository.GetAsync(playerId, ruleset);

        if (currentStats == null)
        {
            return null;
        }

        var matchesPlayed = await matchStatsRepository.CountMatchesPlayedAsync(playerId, ruleset);
        var winRate = await matchStatsRepository.GlobalWinrateAsync(playerId, ruleset);
        var tournamentsPlayed = await tournamentsService.CountPlayedAsync(playerId, ruleset);
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

        return new PlayerRatingStatsDTO
        {
            PlayerId = playerId,
            CountryRank = currentStats.CountryRank,
            GlobalRank = currentStats.GlobalRank,
            MatchesPlayed = matchesPlayed,
            Ruleset = ruleset,
            Percentile = currentStats.Percentile,
            Rating = currentStats.Rating,
            Volatility = currentStats.Volatility,
            WinRate = winRate,
            TournamentsPlayed = tournamentsPlayed,
            RankProgress = rankProgress
        };
    }

    public async Task<IEnumerable<PlayerRatingStatsDTO?>> GetLeaderboardAsync(
        Ruleset ruleset,
        int page,
        int pageSize,
        LeaderboardChartType chartType,
        LeaderboardFilterDTO filter,
        int? playerId
    )
    {
        IEnumerable<PlayerRating> leaderboardRatings = await playerRatingsRepository.GetLeaderboardAsync(
            page,
            pageSize,
            ruleset,
            chartType,
            filter,
            playerId
        );

        var leaderboard = new List<PlayerRatingStatsDTO?>();

        foreach (PlayerRating rating in leaderboardRatings)
        {
            leaderboard.Add(await GetAsync(rating, rating.PlayerId, ruleset));
        }

        return leaderboard;
    }

    public async Task<int> LeaderboardCountAsync(
        Ruleset requestQueryRuleset,
        LeaderboardChartType requestQueryChartType,
        LeaderboardFilterDTO requestQueryFilter,
        int? playerId
    ) =>
        await playerRatingsRepository.LeaderboardCountAsync(
            requestQueryRuleset,
            requestQueryChartType,
            requestQueryFilter,
            playerId
        );

    public async Task<LeaderboardFilterDefaultsDTO> LeaderboardFilterDefaultsAsync(
        Ruleset requestQueryRuleset,
        LeaderboardChartType requestQueryChartType
    ) =>
        new()
        {
            MaxRating = await playerRatingsRepository.HighestRatingAsync(requestQueryRuleset),
            MaxMatches = await playerRatingsRepository.HighestMatchesAsync(requestQueryRuleset),
            MaxRank = 100_000
        };

    public async Task<IDictionary<int, int>> GetHistogramAsync(Ruleset ruleset) =>
        await playerRatingsRepository.GetHistogramAsync(ruleset);
}
