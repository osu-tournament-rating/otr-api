using API.DTOs;
using Common.Enums;

namespace API.Services.Interfaces;

public interface IPlayerRatingsService
{
    Task<PlayerRatingStatsDTO?> GetAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null, bool includeAdjustments = false);

    /// <summary>
    /// See <see cref="IApiPlayerRatingsRepository.GetHistogramAsync"/>
    /// </summary>
    /// <param name="ruleset"></param>
    /// <returns></returns>
    Task<IDictionary<int, int>> GetHistogramAsync(Ruleset ruleset);
}
