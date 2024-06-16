using Database.Entities;
using Database.Entities.Processor;

namespace Database.Repositories.Interfaces;

public interface IGameWinRecordsRepository : IRepository<GameWinRecord>
{
    Task TruncateAsync();
}
