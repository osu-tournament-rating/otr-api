using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

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
