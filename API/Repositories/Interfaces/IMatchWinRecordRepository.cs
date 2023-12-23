using API.DTOs;
using API.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchWinRecordRepository : IRepository<MatchWinRecord>
{
	Task BatchInsertAsync(IEnumerable<MatchWinRecordDTO> postBody);
	Task TruncateAsync();
}