using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Common.Enums;
using Database.Entities.Processor;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class PlayerRatingsService(
    IPlayerRatingsRepository playerRatingsRepository,
    IPlayerMatchStatsRepository matchStatsRepository,
    ITournamentsService tournamentsService,
    IMapper mapper
) : IPlayerRatingsService
{
    public async Task<PlayerRatingStatsDTO?> GetAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null, bool includeAdjustments = false)
    {
        // Note: Adjustments are the only property filtered by time
        PlayerRating? currentStats = await playerRatingsRepository.GetAsync(playerId, ruleset, dateMin, dateMax, includeAdjustments);

        if (currentStats is null)
        {
            return null;
        }

        int matchesPlayed = await matchStatsRepository.CountMatchesPlayedAsync(playerId, ruleset, dateMin, dateMax);
        double winRate = await matchStatsRepository.GlobalWinrateAsync(playerId, ruleset, dateMin, dateMax);
        int tournamentsPlayed = await tournamentsService.CountPlayedAsync(playerId, ruleset, dateMin, dateMax);
        var tierProgress = new TierProgressDTO(currentStats.Rating);

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
            TierProgress = tierProgress,
            Adjustments = mapper.Map<ICollection<RatingAdjustmentDTO>>(currentStats.Adjustments.OrderBy(a => a.Timestamp))
        };
    }

    public async Task<Dictionary<int, PlayerRatingStatsDTO?>> GetAsync(IEnumerable<int> playerIds, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null, bool includeAdjustments = false)
    {
        var playerIdsList = playerIds.ToList();
        var result = new Dictionary<int, PlayerRatingStatsDTO?>();

        // Get all ratings in bulk
        var ratings = await playerRatingsRepository.GetAsync(playerIdsList, ruleset, dateMin, dateMax, includeAdjustments);

        // Get match stats in bulk
        var matchCounts = await matchStatsRepository.CountMatchesPlayedAsync(playerIdsList, ruleset, dateMin, dateMax);
        var winRates = await matchStatsRepository.GlobalWinrateAsync(playerIdsList, ruleset, dateMin, dateMax);

        // Get tournament counts in bulk
        var tournamentCounts = await tournamentsService.CountPlayedAsync(playerIdsList, ruleset, dateMin, dateMax);

        // Build DTOs for each player
        foreach (int playerId in playerIdsList)
        {
            if (!ratings.TryGetValue(playerId, out var currentStats))
            {
                result[playerId] = null;
                continue;
            }

            int matchesPlayed = matchCounts.GetValueOrDefault(playerId, 0);
            double winRate = winRates.GetValueOrDefault(playerId, 0.0);
            int tournamentsPlayed = tournamentCounts.GetValueOrDefault(playerId, 0);
            var tierProgress = new TierProgressDTO(currentStats.Rating);

            result[playerId] = new PlayerRatingStatsDTO
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
                TierProgress = tierProgress,
                Adjustments = includeAdjustments
                    ? mapper.Map<ICollection<RatingAdjustmentDTO>>(currentStats.Adjustments.OrderBy(a => a.Timestamp))
                    : new List<RatingAdjustmentDTO>()
            };
        }

        return result;
    }
}
