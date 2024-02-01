using API.Entities;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations;

public class GamesRepository : RepositoryBase<Game>, IGamesRepository
{
	private readonly OtrContext _context;
	public GamesRepository(OtrContext context) : base(context) { _context = context; }

	public override async Task<int> UpdateAsync(Game game)
	{
		game.Updated = DateTime.UtcNow;
		return await base.UpdateAsync(game);
	}
}