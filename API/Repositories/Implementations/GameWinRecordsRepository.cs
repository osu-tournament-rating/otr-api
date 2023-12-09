using API.Entities;
using API.Repositories.Interfaces;

namespace API.Repositories.Implementations;

public class GameWinRecordsRepository : RepositoryBase<GameWinRecord>, IGameWinRecordsRepository
{
	public GameWinRecordsRepository(OtrContext context) : base(context) {}
}