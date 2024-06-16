using Database.Entities;
using Database.Entities.Processor;

namespace Database.Repositories.Interfaces;

public interface IMatchWinRecordRepository : IRepository<MatchWinRecord>
{
    Task TruncateAsync();
}
