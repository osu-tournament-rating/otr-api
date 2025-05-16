using Common.Enums;
using Common.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Represents platform-wide <see cref="Database.Entities.Tournament"/> stats
/// </summary>
public class TournamentPlatformStatsDTO
{
    /// <summary>
    /// Total number of <see cref="Database.Entities.Tournament"/>s
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Map of <see cref="Common.Enums.Verification.VerificationStatus"/>es to the number of <see cref="Database.Entities.Tournament"/>s with the status
    /// </summary>
    public Dictionary<VerificationStatus, int> CountByVerificationStatus { get; init; } = new();

    /// <summary>
    /// Map of years to the number of verified <see cref="Database.Entities.Tournament"/>s in that year
    /// </summary>
    public Dictionary<int, int> VerifiedByYear { get; init; } = new();

    /// <summary>
    /// Map of <see cref="Common.Enums.Ruleset"/>s to the number of verified <see cref="Database.Entities.Tournament"/>s in that ruleset
    /// </summary>
    public Dictionary<Ruleset, int> VerifiedByRuleset { get; init; } = new();

    /// <summary>
    /// Map of lobby sizes to the number of verified <see cref="Database.Entities.Tournament"/>s with that lobby size
    /// </summary>
    public Dictionary<int, int> VerifiedByLobbySize { get; init; } = new();
}
