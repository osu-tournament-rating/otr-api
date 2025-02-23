using API.DTOs;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiMatchRosterRepository : IMatchRosterRepository
{
    Task<IEnumerable<PlayerFrequencyDTO>> GetFrequentTeammatesAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? maxDate = null,
        int limit = 5
    );

    Task<IEnumerable<PlayerFrequencyDTO>> GetFrequentOpponentsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? maxDate = null,
        int limit = 5
    );
}
