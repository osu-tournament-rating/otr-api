using System.Collections.Immutable;
using System.Text.Json;
using API.DTOs;
using API.Services.Interfaces;
using Common.Enums;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Service for filtering players based on specified criteria
/// </summary>
public class FilteringService(
    IPlayerService playerService,
    IPlayerRatingsService playerRatingsService,
    IPlayerStatsService playerStatsService,
    IPlayersRepository playersRepository,
    IFilterReportsRepository filterReportsRepository) : IFilteringService
{
    public async Task<FilteringResultDTO> FilterAsync(FilteringRequestDTO request, int userId)
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
            var filterResult = await FilterPlayerAsync(request, playerInfo);
            results.Add(filterResult);
        }

        var filteringResult = new FilteringResultDTO
        {
            PlayersPassed = results.Count(r => r.IsSuccess),
            PlayersFailed = results.Count(r => !r.IsSuccess),
            FilteringResults = results.ToList()
        };

        // Store the filter report
        var filterReport = new FilterReport
        {
            UserId = userId,
            RequestJson = JsonSerializer.Serialize(request),
            ResponseJson = "" // Will be updated after we get the ID
        };

        await filterReportsRepository.CreateAsync(filterReport);

        // Set the ID and update the ResponseJson with the complete result
        filteringResult.FilterReportId = filterReport.Id;
        filterReport.ResponseJson = JsonSerializer.Serialize(filteringResult);
        await filterReportsRepository.UpdateAsync(filterReport);

        return filteringResult;
    }

    private async Task<PlayerFilteringResultDTO> FilterPlayerAsync(
        FilteringRequestDTO request,
        PlayerCompactDTO playerInfo)
    {
        var result = new PlayerFilteringResultDTO
        {
            PlayerId = playerInfo.Id,
            Username = playerInfo.Username,
            OsuId = playerInfo.OsuId
        };

        PlayerRatingStatsDTO? ratingStats = await playerRatingsService.GetAsync(
            playerInfo.Id,
            request.Ruleset,
            includeAdjustments: false);

        if (ratingStats is null)
        {
            result.IsSuccess = false;
            result.FailureReason = FilteringFailReason.NoData;
            return result;
        }

        // Populate detailed player data
        result.CurrentRating = ratingStats.Rating;
        result.TournamentsPlayed = ratingStats.TournamentsPlayed;
        result.MatchesPlayed = ratingStats.MatchesPlayed;

        // Separate database call as adjustments are not included from the initial call
        // In the future, the processor could store this field in the database
        double peakRating = await playerStatsService.GetPeakRatingAsync(playerInfo.Id, request.Ruleset);
        result.PeakRating = peakRating;

        // Fetch osu! global rank if rank filtering is requested
        int? globalRank = null;
        if (request.MinRank.HasValue || request.MaxRank.HasValue)
        {
            var playerWithRulesetData = await playersRepository
                .GetWithIncludesAsync(playerInfo.Id, p => p.RulesetData);

            globalRank = playerWithRulesetData?.RulesetData
                .FirstOrDefault(rd => rd.Ruleset == request.Ruleset)?.GlobalRank;
        }
        result.OsuGlobalRank = globalRank;

        FilteringFailReason failReason = EnforceFilteringConditions(request, ratingStats, peakRating, globalRank);
        result.IsSuccess = failReason == FilteringFailReason.None;
        result.FailureReason = failReason == FilteringFailReason.None ? null : failReason;

        return result;
    }

    /// <summary>
    /// Checks all fields of the filter against a player
    /// and applies the appropriate fail reason if needed
    /// </summary>
    /// <param name="request">Filter request</param>
    /// <param name="ratingStats">Rating stats of the player we are checking</param>
    /// <param name="peakRating">The player's all-time peak rating</param>
    /// <param name="globalRank">The player's osu! global rank for the ruleset</param>
    /// <returns></returns>
    private static FilteringFailReason EnforceFilteringConditions(
        FilteringRequestDTO request,
        PlayerRatingStatsDTO ratingStats,
        double peakRating,
        int? globalRank)
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

        // Check osu! global rank constraints
        if (request.MinRank.HasValue && (globalRank == null || globalRank < request.MinRank))
        {
            failReason |= FilteringFailReason.MinRank;
        }

        if (request.MaxRank.HasValue && globalRank.HasValue && globalRank > request.MaxRank)
        {
            failReason |= FilteringFailReason.MaxRank;
        }

        return failReason;
    }
}
