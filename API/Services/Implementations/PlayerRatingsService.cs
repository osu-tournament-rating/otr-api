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

        var matchesPlayed = await matchStatsRepository.CountMatchesPlayedAsync(playerId, ruleset, dateMin, dateMax);
        var winRate = await matchStatsRepository.GlobalWinrateAsync(playerId, ruleset, dateMin, dateMax);
        var tournamentsPlayed = await tournamentsService.CountPlayedAsync(playerId, ruleset, dateMin, dateMax);
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
}
