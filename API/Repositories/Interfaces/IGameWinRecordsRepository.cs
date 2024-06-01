using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IGameWinRecordsRepository : IRepository<GameWinRecord>
{
    Task TruncateAsync();
}
