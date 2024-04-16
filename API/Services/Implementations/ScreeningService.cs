using API.DTOs;
using API.Enums;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class ScreeningService(IPlayerService playerService, IBaseStatsService baseStatsService, IPlayerStatsService
    playerStatsService) : IScreeningService
{
    public async Task<ScreeningResultDTO> ScreenAsync(ScreeningRequestDTO screeningRequest)
    {
        var idList = screeningRequest.OsuPlayerIds.ToList();

        if (idList.Count == 0)
        {
            throw new Exception("Screening id list cannot be empty");
        }

        var passed = 0;
        var failed = 0;

        IEnumerable<PlayerInfoDTO> playerInfoCollection = await playerService.GetAsync(idList);
        var resultCollection = new List<PlayerScreeningResultDTO>();

        foreach (PlayerInfoDTO playerInfo in playerInfoCollection)
        {
            (ScreeningResult result, ScreeningFailReason? failReason) = await ScreenAsync(screeningRequest, playerInfo);

            switch (result)
            {
                case ScreeningResult.Pass:
                    passed++;
                    break;
                case ScreeningResult.Fail:
                    failed++;
                    break;
                default:
                    throw new Exception("This ScreeningResult is not handled!");
            }

            resultCollection.Add(new PlayerScreeningResultDTO
            {
                PlayerId = playerInfo.Id,
                Username = playerInfo.Username,
                OsuId = playerInfo.OsuId,
                ScreeningResult = result,
                ScreeningFailReason = failReason
            });
        }

        return new ScreeningResultDTO
        {
            PlayersPassed = passed,
            PlayersFailed = failed,
            ScreeningResults = resultCollection
        };
    }

    private async Task<(ScreeningResult result, ScreeningFailReason? failReason)> ScreenAsync(ScreeningRequestDTO screeningRequest,
        PlayerInfoDTO? playerInfo)
    {
        ScreeningResult result = ScreeningResult.Fail;
        ScreeningFailReason? failReason = ScreeningFailReason.None;

        if (playerInfo == null)
        {
            failReason |= ScreeningFailReason.NoData;

            return (result, failReason);
        }

        BaseStatsDTO? baseStats = await baseStatsService.GetAsync(null, playerInfo.Id, screeningRequest.Ruleset);

        if (baseStats == null)
        {
            failReason |= ScreeningFailReason.NoData;

            return (result, failReason);
        }

        if (baseStats.Rating < screeningRequest.MinRating)
        {
            failReason |= ScreeningFailReason.MinRating;
        }

        if (baseStats.Rating > screeningRequest.MaxRating)
        {
            failReason |= ScreeningFailReason.MaxRating;
        }

        if (!screeningRequest.AllowProvisional && baseStats.IsProvisional)
        {
            failReason |= ScreeningFailReason.IsProvisional;
        }

        if (baseStats.MatchesPlayed < screeningRequest.MatchesPlayed)
        {
            failReason |= ScreeningFailReason.NotEnoughMatches;
        }

        if (baseStats.TournamentsPlayed < screeningRequest.TournamentsPlayed)
        {
            failReason |= ScreeningFailReason.NotEnoughTournaments;
        }

        var peakRating = await playerStatsService.GetPeakRatingAsync(playerInfo.Id, screeningRequest.Ruleset);
        if (peakRating > screeningRequest.PeakRating)
        {
            failReason |= ScreeningFailReason.PeakRatingTooHigh;
        }

        if (failReason == ScreeningFailReason.None)
        {
            result = ScreeningResult.Pass;
        }

        return (result, failReason);
    }
}
