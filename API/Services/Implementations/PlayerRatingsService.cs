using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;
using Common.Enums;
using Database.Entities.Processor;
using Database.Models;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class PlayerRatingsService(
    IPlayerRatingsRepository playerRatingsRepository,
    IPlayerMatchStatsRepository matchStatsRepository,
    IPlayersRepository playersRepository,
    ITournamentsService tournamentsService,
    IMapper mapper
) : IPlayerRatingsService
{
    public async Task<IEnumerable<PlayerRatingStatsDTO?>> GetAsync(long osuPlayerId)
    {
        var id = await playersRepository.GetIdAsync(osuPlayerId);

        if (!id.HasValue)
        {
            return [];
        }

        IList<Ruleset> activeRulesets = await playerRatingsRepository.GetActiveRulesetsAsync(id.Value);
        var allRulesetRatings = new List<PlayerRatingStatsDTO?>();

        foreach (Ruleset ruleset in activeRulesets)
        {
            // One per ruleset
            allRulesetRatings.Add(await GetAsync(id.Value, ruleset, true));
        }

        return allRulesetRatings;
    }

    public async Task<PlayerRatingStatsDTO?> GetAsync(int playerId, Ruleset ruleset, bool includeAdjustments)
    {
        PlayerRating? currentStats = await playerRatingsRepository.GetAsync(playerId, ruleset, includeAdjustments);

        if (currentStats is null)
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
            Player = mapper.Map<PlayerCompactDTO>(currentStats.Player),
            CountryRank = currentStats.CountryRank,
            GlobalRank = currentStats.GlobalRank,
            MatchesPlayed = matchesPlayed,
            Ruleset = ruleset,
            Percentile = currentStats.Percentile,
            Rating = currentStats.Rating,
            Volatility = currentStats.Volatility,
            WinRate = winRate,
            TournamentsPlayed = tournamentsPlayed,
            RankProgress = rankProgress,
            Adjustments = mapper.Map<ICollection<RatingAdjustmentDTO>>(currentStats.Adjustments.OrderBy(a => a.Timestamp))
        };
    }

    public async Task<LeaderboardDTO> GetLeaderboardAsync(
        LeaderboardRequestQueryDTO request
    )
    {
        LeaderboardFilter? lbFilter = mapper.Map<LeaderboardFilter>(request.Filter);
        IEnumerable<PlayerRating> leaderboardRatings = await playerRatingsRepository.GetLeaderboardAsync(
            request.Page,
            request.PageSize,
            request.Ruleset,
            request.ChartType,
            lbFilter,
            request.Country
        );

        var ratingStats = new List<PlayerRatingStatsDTO>();
        foreach (PlayerRating rating in leaderboardRatings)
        {
            PlayerRatingStatsDTO? ratingStat = await GetAsync(rating.PlayerId, request.Ruleset, false);

            if (ratingStat is not null)
            {
                ratingStats.Add(ratingStat);
            }
        }

        var counts = await LeaderboardCountAsync(request.Ruleset, request.ChartType, lbFilter, request.Country);
        LeaderboardFilterDefaultsDTO defaults = await LeaderboardFilterDefaultsAsync(request.Ruleset);

        return new LeaderboardDTO
        {
            Ruleset = request.Ruleset,
            TotalPlayerCount = counts,
            FilterDefaults = defaults,
            Leaderboard = ratingStats
        };
    }

    private async Task<int> LeaderboardCountAsync(
        Ruleset ruleset,
        LeaderboardChartType requestQueryChartType,
        LeaderboardFilter requestQueryFilter,
        string? country
    ) =>
        await playerRatingsRepository.LeaderboardCountAsync(
            ruleset,
            requestQueryChartType,
            requestQueryFilter,
            country
        );

    private async Task<LeaderboardFilterDefaultsDTO> LeaderboardFilterDefaultsAsync(
        Ruleset requestQueryRuleset
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
