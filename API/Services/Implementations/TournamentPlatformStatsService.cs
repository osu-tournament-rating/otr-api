using API.DTOs;
using API.Services.Interfaces;
using Common.Constants;
using Common.Enums;
using Common.Enums.Verification;
using Common.Utilities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class TournamentPlatformStatsService(ITournamentsRepository tournamentsRepository) : ITournamentPlatformStatsService
{
    public async Task<TournamentPlatformStatsDTO> GetAsync()
    {
        Dictionary<VerificationStatus, int> countsByStatuses = await tournamentsRepository.GetVerificationStatusStatsAsync();
        ImputeMissingKeys(countsByStatuses, EnumUtils.MinValue<VerificationStatus>(), EnumUtils.MaxValue<VerificationStatus>());

        Dictionary<int, int> countsByYears = await tournamentsRepository.GetYearStatsAsync();
        ImputeMissingKeys(countsByYears);

        Dictionary<Ruleset, int> countsByRulesets = await tournamentsRepository.GetRulesetStatsAsync();
        ImputeMissingKeys(countsByRulesets, EnumUtils.MinValue<Ruleset>(), EnumUtils.MaxValue<Ruleset>());

        Dictionary<int, int> countsByLobbySizes = await tournamentsRepository.GetLobbySizeStatsAsync();
        ImputeMissingKeys(countsByLobbySizes, LobbySizeConstants.MinValue, LobbySizeConstants.MaxValue);

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

    private static void ImputeMissingKeys<T>(Dictionary<T, int> counts, T? minValue = null, T? maxValue = null) where T : struct, IConvertible
    {
        var start = (minValue ?? counts.Keys.Min()).ToInt32(null);
        var end = (maxValue ?? counts.Keys.Max()).ToInt32(null);

        for (var i = start; i <= end; i++)
        {
            counts.TryAdd((T)(object)i, 0);
        }
    }
}
