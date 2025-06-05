using API.DTOs;
using Common.Enums;

namespace API.Services.Interfaces;

public interface IPlayerRatingsService
{
    Task<PlayerRatingStatsDTO?> GetAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null, bool includeAdjustments = false);
}
