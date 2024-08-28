using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IGameWinRecordsRepository : IRepository<GameWinRecord>
{
    Task TruncateAsync();
}
