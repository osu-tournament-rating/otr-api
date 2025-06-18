using Common.Enums;
using Common.Enums.Verification;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents platform-wide <see cref="Database.Entities.Tournament"/> stats
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class TournamentPlatformStatsDTO
{
    /// <summary>
    /// Total number of <see cref="Database.Entities.Tournament"/>s
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Map of <see cref="Common.Enums.Verification.VerificationStatus"/>es to the number of <see cref="Database.Entities.Tournament"/>s with the status
    /// </summary>
    public IDictionary<VerificationStatus, int> CountByVerificationStatus { get; init; } = new Dictionary<VerificationStatus, int>();

    /// <summary>
    /// Map of years to the number of verified <see cref="Database.Entities.Tournament"/>s in that year
    /// </summary>
    public IDictionary<int, int> VerifiedByYear { get; init; } = new Dictionary<int, int>();

    /// <summary>
    /// Map of <see cref="Common.Enums.Ruleset"/>s to the number of verified <see cref="Database.Entities.Tournament"/>s in that ruleset
    /// </summary>
    public IDictionary<Ruleset, int> VerifiedByRuleset { get; init; } = new Dictionary<Ruleset, int>();

    /// <summary>
    /// Map of lobby sizes to the number of verified <see cref="Database.Entities.Tournament"/>s with that lobby size
    /// </summary>
    public IDictionary<int, int> VerifiedByLobbySize { get; init; } = new Dictionary<int, int>();
}
