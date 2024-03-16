using System.Diagnostics.CodeAnalysis;
using API.Entities;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class GamesRepository(OtrContext context) : RepositoryBase<Game>(context), IGamesRepository
{
    public override async Task<int> UpdateAsync(Game game)
    {
        game.Updated = DateTime.UtcNow;
        return await base.UpdateAsync(game);
    }
}
