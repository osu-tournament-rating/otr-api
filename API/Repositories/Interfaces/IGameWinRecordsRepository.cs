using API.DTOs;
using API.Entities;

namespace API.Repositories.Interfaces;

public interface IGameWinRecordsRepository : IRepository<GameWinRecord>
{
    Task BatchInsertAsync(IEnumerable<GameWinRecordDTO> postBody);
    Task TruncateAsync();
}
