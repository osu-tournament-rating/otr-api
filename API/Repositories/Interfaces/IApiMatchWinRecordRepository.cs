using API.DTOs;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiMatchWinRecordRepository : IMatchWinRecordRepository
{
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
