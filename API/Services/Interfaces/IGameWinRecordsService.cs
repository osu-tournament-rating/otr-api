using API.DTOs;

namespace API.Services.Interfaces;

public interface IGameWinRecordsService
{
    Task BatchInsertAsync(IEnumerable<GameWinRecordDTO> postBody);
    Task TruncateAsync();
}
