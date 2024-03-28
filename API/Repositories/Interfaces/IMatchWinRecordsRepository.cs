using API.DTOs;
using API.Entities;

namespace API.Repositories.Interfaces;

public interface IMatchWinRecordsRepository : IRepository<MatchWinRecord>
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
