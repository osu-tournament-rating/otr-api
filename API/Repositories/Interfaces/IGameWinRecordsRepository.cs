using Database.Entities;

namespace API.Repositories.Interfaces;

public interface IGameWinRecordsRepository : IRepository<GameWinRecord>
{
    Task TruncateAsync();
}
