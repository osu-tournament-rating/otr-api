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

        var resultCollection = new List<PlayerScreeningResultDTO>();
        foreach (var osuId in idList)
        {
            PlayerInfoDTO? playerInfo = await playerService.GetAsync(osuId);
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
                PlayerId = playerInfo?.Id,
                Username = playerInfo?.Username,
                OsuPlayerId = osuId,
                ScreeningResult = result,
                ScreeningFailReason = failReason
            });
        }

        var dto = new ScreeningResultDTO
        {
            PlayersPassed = passed,
            PlayersFailed = failed,
            ScreeningResults = resultCollection
        };

        return dto;
    }

    private async Task<(ScreeningResult result, ScreeningFailReason? failReason)> ScreenAsync(ScreeningRequestDTO screeningRequest,
        PlayerInfoDTO? playerInfo)
    {
        ScreeningResult result = ScreeningResult.Pass;
        ScreeningFailReason? failReason = ScreeningFailReason.None;

        if (playerInfo == null)
        {
            result = ScreeningResult.Fail;
            failReason |= ScreeningFailReason.NoData;

            return (result, failReason);
        }


        BaseStatsDTO? baseStats = await baseStatsService.GetAsync(null, playerInfo.Id, screeningRequest.Ruleset);
        var peakRating = await playerStatsService.GetPeakRatingAsync(playerInfo.Id, screeningRequest.Ruleset);


        if (baseStats == null)
        {
            result = ScreeningResult.Fail;
            failReason |= ScreeningFailReason.NoData;

            return (result, failReason);
        }

        if (baseStats.Rating < screeningRequest.MinRating)
        {
            result = ScreeningResult.Fail;
            failReason |= ScreeningFailReason.MinRating;
        }

        if (baseStats.Rating > screeningRequest.MaxRating)
        {
            result = ScreeningResult.Fail;
            failReason |= ScreeningFailReason.MaxRating;
        }

        if (!screeningRequest.AllowProvisional && baseStats.IsProvisional)
        {
            result = ScreeningResult.Fail;
            failReason |= ScreeningFailReason.IsProvisional;
        }

        if (baseStats.MatchesPlayed < screeningRequest.MatchesPlayed)
        {
            result = ScreeningResult.Fail;
            failReason |= ScreeningFailReason.NotEnoughMatches;
        }

        if (baseStats.TournamentsPlayed < screeningRequest.TournamentsPlayed)
        {
            result = ScreeningResult.Fail;
            failReason |= ScreeningFailReason.NotEnoughTournaments;
        }

        if (peakRating > screeningRequest.PeakRating)
        {
            result = ScreeningResult.Fail;
            failReason |= ScreeningFailReason.PeakRatingTooHigh;
        }

        return (result, failReason);
    }
}
