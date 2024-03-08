using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class GameWinRecordsRepository(OtrContext context) : RepositoryBase<GameWinRecord>(context), IGameWinRecordsRepository
{
    private readonly OtrContext _context = context;

    public async Task BatchInsertAsync(IEnumerable<GameWinRecordDTO> postBody)
    {
        foreach (GameWinRecordDTO item in postBody)
        {
            var record = new GameWinRecord
            {
                GameId = item.GameId,
                Winners = item.Winners,
                Losers = item.Losers,
                WinnerTeam = item.WinnerTeam,
                LoserTeam = item.LoserTeam
            };

            await _context.AddAsync(record);
        }

        await _context.SaveChangesAsync();
    }

    public async Task TruncateAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE game_win_records RESTART IDENTITY");
        await _context.SaveChangesAsync();
    }
}
