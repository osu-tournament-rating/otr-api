using API.DTOs;
using API.Services.Interfaces;
using Common.Enums.Verification;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class TournamentPlatformStatsService(ITournamentsRepository tournamentsRepository) : ITournamentPlatformStatsService
{
    public async Task<TournamentPlatformStatsDTO> GetAsync()
    {
        Dictionary<VerificationStatus, int> countsByStatuses = await tournamentsRepository.GetVerificationStatusStatsAsync();
        var totalCount = countsByStatuses.Sum(x => x.Value);

        return new TournamentPlatformStatsDTO
        {
            TotalCount = totalCount,
            CountsByVerificationStatuses = countsByStatuses
        };
    }
}
