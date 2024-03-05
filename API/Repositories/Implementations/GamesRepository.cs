using API.Entities;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations;

public class GamesRepository : RepositoryBase<Game>, IGamesRepository
{
    public GamesRepository(OtrContext context)
        : base(context) { }

    public override async Task<int> UpdateAsync(Game game)
    {
        game.Updated = DateTime.UtcNow;
        return await base.UpdateAsync(game);
    }
}
