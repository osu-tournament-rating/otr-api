using API.DTOs;
using API.Services.Interfaces;
using Common.Enums;
using Common.Enums.Verification;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class TournamentPlatformStatsService(ITournamentsRepository tournamentsRepository) : ITournamentPlatformStatsService
{
    public async Task<TournamentPlatformStatsDTO> GetAsync()
    {
        Dictionary<VerificationStatus, int> countsByStatuses = await tournamentsRepository.GetVerificationStatusStatsAsync();
        Dictionary<int, int> countsByYears = await tournamentsRepository.GetYearStatsAsync();
        Dictionary<Ruleset, int> countsByRulesets = await tournamentsRepository.GetRulesetStatsAsync();
        Dictionary<int, int> countsByLobbySizes = await tournamentsRepository.GetLobbySizeStatsAsync();

        var totalCount = countsByStatuses.Sum(x => x.Value);

        return new TournamentPlatformStatsDTO
        {
            TotalCount = totalCount,
            CountByVerificationStatus = countsByStatuses,
            VerifiedByYear = countsByYears,
            VerifiedByRuleset = countsByRulesets,
            VerifiedByLobbySize = countsByLobbySizes,
        };
    }
}
