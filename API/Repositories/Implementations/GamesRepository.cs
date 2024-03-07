using API.Entities;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations;

public class GamesRepository(OtrContext context) : RepositoryBase<Game>(context), IGamesRepository
{
    public override async Task<int> UpdateAsync(Game game)
    {
        game.Updated = DateTime.UtcNow;
        return await base.UpdateAsync(game);
    }
}
