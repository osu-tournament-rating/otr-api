using Common.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Represents statistics on all <see cref="Database.Entities.Tournament"/>s existing in a system
/// </summary>
public class TournamentPlatformStatsDTO
{
    /// <summary>
    /// Total number of all <see cref="Database.Entities.Tournament"/>s in the system
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Mapping of <see cref="Common.Enums.Verification.VerificationStatus"/>es to number of <see cref="Database.Entities.Tournament"/>s having corresponding status
    /// </summary>
    public Dictionary<VerificationStatus, int> CountsByVerificationStatuses { get; init; } = new();
}
