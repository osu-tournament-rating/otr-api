using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IMatchWinRecordRepository : IRepository<MatchWinRecord>
{
    Task TruncateAsync();
}
