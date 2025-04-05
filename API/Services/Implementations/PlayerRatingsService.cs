using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
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

    public async Task<IDictionary<int, int>> GetHistogramAsync(Ruleset ruleset) =>
        await playerRatingsRepository.GetHistogramAsync(ruleset);
}
