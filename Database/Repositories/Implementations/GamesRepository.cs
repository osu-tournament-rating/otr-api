using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class GamesRepository(OtrContext context) : RepositoryBase<Game>(context), IGamesRepository
{
    private readonly OtrContext _context = context;

    public async Task<Game?> GetAsync(long osuId) =>
        LocalView.FirstOrDefault(g => g.OsuId == osuId)
        ?? await _context.Games.FirstOrDefaultAsync(g => g.OsuId == osuId);
}
