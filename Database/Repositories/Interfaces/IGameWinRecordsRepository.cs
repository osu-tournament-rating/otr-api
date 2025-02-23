using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IGameWinRecordsRepository : IRepository<GameRoster>
{
    Task TruncateAsync();
}
