using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance",
    "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class GamesRepository(OtrContext context) : RepositoryBase<Game>(context), IGamesRepository
{
    public async Task<Game?> GetAsync(int id, bool verified) =>
        verified
            ? await context.Games
                .WhereVerified()
                .IncludeChildren(verified)
                .FirstOrDefaultAsync(g => g.Id == id)
            : await context.Games
                .IncludeChildren(verified)
                .Where(g => g.Id == id)
                .FirstOrDefaultAsync();
}
