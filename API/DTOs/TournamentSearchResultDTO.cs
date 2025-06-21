using Common.Enums;
using Common.Enums.Verification;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a search result for a tournament
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class TournamentSearchResultDTO
{
    /// <summary>
    /// Id of the tournament
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Ruleset of the tournament
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Verification status of the tournament
    /// </summary>
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// Rejection reason of the tournament
    /// </summary>
    public TournamentRejectionReason RejectionReason { get; set; }

    /// <summary>
    /// Abbreviation of the tournament
    /// </summary>
    public string? Abbreviation { get; set; }

    /// <summary>
    /// Expected in-match team size
    /// </summary>
    public int LobbySize { get; set; }

    /// <summary>
    /// Name of the tournament
    /// </summary>
    public required string Name { get; set; }
}
