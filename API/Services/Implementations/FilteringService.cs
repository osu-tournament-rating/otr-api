using API.DTOs;
using API.Services.Interfaces;
using Database.Enums;

namespace API.Services.Implementations;

public class FilteringService(IPlayerService playerService, IPlayerRatingService playerRatingService, IPlayerStatsService
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

    private async Task<(FilteringResult result, FilteringFailReason? failReason)> FilterAsync(FilteringRequestDTO filteringRequest,
        PlayerCompactDTO? playerInfo)
    {
        FilteringResult result = FilteringResult.Fail;
        FilteringFailReason? failReason = FilteringFailReason.None;

        if (playerInfo == null)
        {
            failReason |= FilteringFailReason.NoData;

            return (result, failReason);
        }

        PlayerRatingStatsDTO? baseStats = await playerRatingService.GetAsync(null, playerInfo.Id, filteringRequest.Ruleset);

        if (baseStats == null)
        {
            failReason |= FilteringFailReason.NoData;

            return (result, failReason);
        }

        if (baseStats.Rating < filteringRequest.MinRating)
        {
            failReason |= FilteringFailReason.MinRating;
        }

        if (baseStats.Rating > filteringRequest.MaxRating)
        {
            failReason |= FilteringFailReason.MaxRating;
        }

        if (!filteringRequest.AllowProvisional && baseStats.IsProvisional)
        {
            failReason |= FilteringFailReason.IsProvisional;
        }

        if (baseStats.MatchesPlayed < filteringRequest.MatchesPlayed)
        {
            failReason |= FilteringFailReason.NotEnoughMatches;
        }

        if (baseStats.TournamentsPlayed < filteringRequest.TournamentsPlayed)
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
