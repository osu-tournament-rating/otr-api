using API.DTOs;
using Database.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchWinRecordRepository : IRepository<MatchWinRecord>
{
    Task BatchInsertAsync(IEnumerable<MatchWinRecordDTO> postBody);
    Task TruncateAsync();

    Task<IEnumerable<PlayerFrequencyDTO>> GetFrequentTeammatesAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? maxDate = null,
        int limit = 5
    );

    Task<IEnumerable<PlayerFrequencyDTO>> GetFrequentOpponentsAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? maxDate = null,
        int limit = 5
    );
}
