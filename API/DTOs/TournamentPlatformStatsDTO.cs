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
    public Dictionary<VerificationStatus, int> CountsByVerificationStatuses { get; init; } = new();
}
