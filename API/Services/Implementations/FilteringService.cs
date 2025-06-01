using System.Collections.Immutable;
using API.DTOs;
using API.Services.Interfaces;
using Common.Enums;

namespace API.Services.Implementations;

/// <summary>
/// Service for filtering players based on specified criteria
/// </summary>
public class FilteringService(
    IPlayerService playerService,
    IPlayerRatingsService playerRatingsService,
    IPlayerStatsService
        playerStatsService) : IFilteringService
{
    public async Task<FilteringResultDTO> FilterAsync(FilteringRequestDTO request)
    {
        var osuIdHashSet = request.OsuPlayerIds.ToImmutableHashSet();
        IEnumerable<PlayerCompactDTO> players = (await playerService.GetAsync(osuIdHashSet)).ToList();

        // Store missing players first
        var results = osuIdHashSet
            .Except(players.Select(p => p.OsuId))
            .Select(osuId => new PlayerFilteringResultDTO
            {
                OsuId = osuId,
                IsSuccess = false,
                FailureReason = FilteringFailReason.NoData
            }).ToList();

        // ReSharper disable once SimplifyLinqExpressionUseAll
        // Iterate all players which are not already stored as missing
        foreach (PlayerCompactDTO playerInfo in players.Where(p => !results.Any(r => r.OsuId == p.OsuId)))
        {
            FilteringFailReason failReason = await FilterPlayerAsync(request, playerInfo);

            results.Add(new PlayerFilteringResultDTO
            {
                PlayerId = playerInfo.Id,
                Username = playerInfo.Username,
                OsuId = playerInfo.OsuId,
                IsSuccess = failReason == FilteringFailReason.None,
                FailureReason = failReason
            });
        }

        return new FilteringResultDTO
        {
            PlayersPassed = results.Count(r => r.IsSuccess),
            PlayersFailed = results.Count(r => !r.IsSuccess),
            FilteringResults = results.ToList()
        };
    }

    private async Task<FilteringFailReason> FilterPlayerAsync(
        FilteringRequestDTO request,
        PlayerCompactDTO playerInfo)
    {
        PlayerRatingStatsDTO? ratingStats = await playerRatingsService.GetAsync(
            playerInfo.Id,
            request.Ruleset,
            includeAdjustments: false);

        if (ratingStats is null)
        {
            return FilteringFailReason.NoData;
        }

        // Separate database call as adjustments are not included from the initial call
        // In the future, the processor could store this field in the database
        double peakRating = await playerStatsService.GetPeakRatingAsync(playerInfo.Id, request.Ruleset);
        FilteringFailReason failReason = EnforceFilteringConditions(request, ratingStats, peakRating);

        return failReason;
    }

    /// <summary>
    /// Checks all fields of the filter against a player
    /// and applies the appropriate fail reason if needed
    /// </summary>
    /// <param name="request">Filter request</param>
    /// <param name="ratingStats">Rating stats of the player we are checking</param>
    /// <param name="peakRating">The player's all-time peak rating</param>
    /// <returns></returns>
    private static FilteringFailReason EnforceFilteringConditions(
        FilteringRequestDTO request,
        PlayerRatingStatsDTO ratingStats,
        double peakRating)
    {
        FilteringFailReason failReason = FilteringFailReason.None;

        if (ratingStats.Rating < request.MinRating)
        {
            failReason |= FilteringFailReason.MinRating;
        }

        if (ratingStats.Rating > request.MaxRating)
        {
            failReason |= FilteringFailReason.MaxRating;
        }

        if (!request.AllowProvisional && ratingStats.IsProvisional)
        {
            failReason |= FilteringFailReason.IsProvisional;
        }

        if (ratingStats.TournamentsPlayed < request.TournamentsPlayed)
        {
            failReason |= FilteringFailReason.NotEnoughTournaments;
        }

        if (peakRating > request.PeakRating)
        {
            failReason |= FilteringFailReason.PeakRatingTooHigh;
        }

        if (ratingStats.MatchesPlayed < request.MatchesPlayed)
        {
            failReason |= FilteringFailReason.NotEnoughMatches;
        }

        return failReason;
    }
}
