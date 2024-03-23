using API.DTOs;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class ScreeningService(IPlayerService playerService, IBaseStatsService baseStatsService) : IScreeningService
{
    public async Task<IEnumerable<ScreeningResultDTO>> ScreenAsync(ScreeningDTO screeningRequest)
    {
        var idList = screeningRequest.OsuPlayerIds.ToList();

        if (idList.Count == 0)
        {
            throw new Exception("Screening id list cannot be empty");
        }

        var resultCollection = new List<ScreeningResultDTO>();
        foreach (var osuId in idList)
        {
            PlayerInfoDTO? playerInfo = await playerService.GetAsync(osuId);
            (ScreeningResult result, ScreeningFailReason? failReason) = ScreenAsync(screeningRequest, playerInfo);

            resultCollection.Add(new ScreeningResultDTO
            {
                PlayerId = playerInfo?.Id,
                OsuPlayerId = osuId,
                ScreeningResult = result,
                ScreeningFailReason = failReason
            });
        }

        return resultCollection;
    }

    public (ScreeningResult result, ScreeningFailReason? failReason) ScreenAsync(ScreeningDTO screeningRequest,
        PlayerInfoDTO? playerInfo)
    {
        ScreeningResult result = ScreeningResult.Pass;
        ScreeningFailReason? failReason = ScreeningFailReason.None;

        if (playerInfo == null)
        {
            result = ScreeningResult.Fail;
            failReason |= ScreeningFailReason.NoData;
        }

        return (result, failReason);
    }
}
