using System.ComponentModel.DataAnnotations;

namespace DWS.Messages;

/// <summary>
/// Message used to request processing automation checks for a match.
/// </summary>
public record ProcessMatchAutomationCheckMessage : Message
{
    /// <summary>
    /// The match ID to process automation checks for.
    /// </summary>
    [Required]
    public int MatchId { get; init; }

    /// <summary>
    /// Whether to override existing human-verified or rejected states.
    /// When true, automation checks will run even on entities that have been manually verified or rejected.
    /// </summary>
    public bool OverrideVerifiedState { get; init; }
}
