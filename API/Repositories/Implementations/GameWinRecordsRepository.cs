using System.Diagnostics.CodeAnalysis;
using API.Repositories.Interfaces;
using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class GameWinRecordsRepository(OtrContext context) : RepositoryBase<GameWinRecord>(context), IGameWinRecordsRepository
{
    private readonly OtrContext _context = context;

    public async Task TruncateAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE game_win_records RESTART IDENTITY");
        await _context.SaveChangesAsync();
    }
}
