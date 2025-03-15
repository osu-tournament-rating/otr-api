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
        var idList = filteringRequest.OsuPlayerIds.ToList();

        if (idList.Count == 0)
        {
            throw new Exception("Filtering id list cannot be empty");
        }

        var passed = 0;
        var failed = 0;

        IEnumerable<PlayerCompactDTO> playerInfoCollection = await playerService.GetAsync(idList);
        var resultCollection = new List<PlayerFilteringResultDTO>();

        foreach (PlayerCompactDTO playerInfo in playerInfoCollection)
        {
            (FilteringResult result, FilteringFailReason? failReason) = await FilterAsync(filteringRequest, playerInfo);

            switch (result)
            {
                case FilteringResult.Pass:
                    passed++;
                    break;
                case FilteringResult.Fail:
                    failed++;
                    break;
                default:
                    throw new Exception("This FilteringResult is not handled!");
            }

            resultCollection.Add(new PlayerFilteringResultDTO
            {
                PlayerId = playerInfo.Id,
                Username = playerInfo.Username,
                OsuId = playerInfo.OsuId,
                FilteringResult = result,
                FilteringFailReason = failReason
            });
        }

        return new FilteringResultDTO
        {
            PlayersPassed = passed,
            PlayersFailed = failed,
            FilteringResults = resultCollection
        };
    }

    private async Task<(FilteringResult result, FilteringFailReason? failReason)> FilterAsync(
        FilteringRequestDTO filteringRequest,
        PlayerCompactDTO? playerInfo)
    {
        FilteringResult result = FilteringResult.Fail;
        FilteringFailReason? failReason = FilteringFailReason.None;

        if (playerInfo == null)
        {
            failReason |= FilteringFailReason.NoData;

            return (result, failReason);
        }

        PlayerRatingStatsDTO? ratingStats =
            await playerRatingsService.GetAsync(playerInfo.Id, filteringRequest.Ruleset, false);

        if (ratingStats == null)
        {
            failReason |= FilteringFailReason.NoData;

            return (result, failReason);
        }

        if (ratingStats.Rating < filteringRequest.MinRating)
        {
            failReason |= FilteringFailReason.MinRating;
        }

        if (ratingStats.Rating > filteringRequest.MaxRating)
        {
            failReason |= FilteringFailReason.MaxRating;
        }

        if (!filteringRequest.AllowProvisional && ratingStats.IsProvisional)
        {
            failReason |= FilteringFailReason.IsProvisional;
        }

        if (ratingStats.MatchesPlayed < filteringRequest.MatchesPlayed)
        {
            failReason |= FilteringFailReason.NotEnoughMatches;
        }

        if (ratingStats.TournamentsPlayed < filteringRequest.TournamentsPlayed)
        {
            failReason |= FilteringFailReason.NotEnoughTournaments;
        }

        var peakRating = await playerStatsService.GetPeakRatingAsync(playerInfo.Id, filteringRequest.Ruleset);
        if (peakRating > filteringRequest.PeakRating)
        {
            failReason |= FilteringFailReason.PeakRatingTooHigh;
        }

        if (failReason == FilteringFailReason.None)
        {
            result = FilteringResult.Pass;
        }

        return (result, failReason);
    }
}
