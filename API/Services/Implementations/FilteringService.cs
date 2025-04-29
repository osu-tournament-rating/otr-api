using API.DTOs;
using API.Services.Interfaces;
using Common.Enums;

namespace API.Services.Implementations;

public class FilteringService(
    IPlayerService playerService,
    IPlayerRatingsService playerRatingsService,
    IPlayerStatsService
        playerStatsService) : IFilteringService
{
    public async Task<FilteringResultDTO> FilterAsync(FilteringRequestDTO filteringRequest)
    {
        ArgumentNullException.ThrowIfNull(filteringRequest);

        var idList = filteringRequest.OsuPlayerIds.ToList();

        if (idList.Count == 0)
        {
            throw new ArgumentException("Filtering id list cannot be empty", nameof(filteringRequest));
        }

        IEnumerable<PlayerCompactDTO?> players = await playerService.GetAsync(idList);

        var results = new List<PlayerFilteringResultDTO>();

        foreach (PlayerCompactDTO? playerInfo in players)
        {
            PlayerFilteringResultDTO result = await ProcessPlayerAsync(playerInfo, filteringRequest);
            results.Add(result);
        }

        // Count results and create result DTO
        var passed = results.Count(r => r.FilteringResult == FilteringResult.Pass);
        var failed = results.Count(r => r.FilteringResult == FilteringResult.Fail);

        return new FilteringResultDTO
        {
            PlayersPassed = passed,
            PlayersFailed = failed,
            FilteringResults = results.ToList()
        };
    }

    private async Task<PlayerFilteringResultDTO> ProcessPlayerAsync(
        PlayerCompactDTO? playerInfo,
        FilteringRequestDTO request)
    {
        (FilteringResult result, FilteringFailReason? failReason) = await FilterAsync(request, playerInfo);

        return new PlayerFilteringResultDTO
        {
            PlayerId = playerInfo?.Id,
            Username = playerInfo?.Username,
            OsuId = playerInfo?.OsuId ?? 0,
            FilteringResult = result,
            FilteringFailReason = failReason
        };
    }

    private async Task<(FilteringResult result, FilteringFailReason? failReason)> FilterAsync(
        FilteringRequestDTO request,
        PlayerCompactDTO? playerInfo)
    {
        // Handle null player info case
        if (playerInfo == null)
        {
            return (FilteringResult.Fail, FilteringFailReason.NoData);
        }

        PlayerRatingStatsDTO? ratingStats = await playerRatingsService.GetAsync(
            playerInfo.Id,
            request.Ruleset,
            includeAdjustments: false);

        if (ratingStats == null)
        {
            return (FilteringResult.Fail, FilteringFailReason.NoData);
        }

        FilteringFailReason failReason = CheckFilteringConditions(request, ratingStats);

        var peakRating = await playerStatsService.GetPeakRatingAsync(playerInfo.Id, request.Ruleset);
        if (peakRating > request.PeakRating)
        {
            failReason |= FilteringFailReason.PeakRatingTooHigh;
        }

        FilteringResult result = failReason == FilteringFailReason.None ? FilteringResult.Pass : FilteringResult.Fail;

        return (result, failReason);
    }

    private static FilteringFailReason CheckFilteringConditions(
        FilteringRequestDTO request,
        PlayerRatingStatsDTO ratingStats)
    {
        FilteringFailReason failReason = FilteringFailReason.None;

        // Apply all basic filtering conditions
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

        if (ratingStats.MatchesPlayed < request.MatchesPlayed)
        {
            failReason |= FilteringFailReason.NotEnoughMatches;
        }

        if (ratingStats.TournamentsPlayed < request.TournamentsPlayed)
        {
            failReason |= FilteringFailReason.NotEnoughTournaments;
        }

        return failReason;
    }
}
