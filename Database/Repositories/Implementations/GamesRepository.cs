using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

/// <summary>
/// Repository for managing <see cref="Game"/> entities
/// </summary>
[SuppressMessage("Performance",
    "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class GamesRepository(OtrContext context) : RepositoryBase<Game>(context), IGamesRepository
{
    private readonly OtrContext _context = context;

    public async Task<Game?> GetAsync(int id, bool verified) =>
        await _context.Games
            .AsNoTracking()
            .IncludeChildren(verified)
            .FirstOrDefaultAsync(g => g.Id == id);

    public async Task LoadScoresAsync(Game game)
    {
        await _context.Entry(game)
            .Collection(g => g.Scores)
            .LoadAsync();
    }
}
