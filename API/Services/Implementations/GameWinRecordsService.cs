using API.DTOs;
using API.Services.Interfaces;
using Database.Entities;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class GameWinRecordsService(IGameWinRecordsRepository gameWinRecordsRepository) : IGameWinRecordsService
{
    public async Task BatchInsertAsync(IEnumerable<GameWinRecordDTO> postBody)
    {
        var gameWinRecords = postBody.Select(item => new GameWinRecord
        {
            GameId = item.GameId,
            WinnerRoster = item.WinnerRoster,
            LoserRoster = item.LoserRoster,
            WinnerTeam = (Team)item.WinnerTeam,
            LoserTeam = (Team)item.LoserTeam
        })
            .ToList();

        await gameWinRecordsRepository.BulkInsertAsync(gameWinRecords);
    }

    public async Task TruncateAsync() => await gameWinRecordsRepository.TruncateAsync();
}
