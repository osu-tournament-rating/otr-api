using System.ComponentModel.DataAnnotations;

namespace DWS.Messages;

/// <summary>
/// Message used to request processing automation checks for a score.
/// </summary>
public record ProcessScoreAutomationCheckMessage : Message
{
    /// <summary>
    /// The score ID to process automation checks for.
    /// </summary>
    [Required]
    public int ScoreId { get; init; }

    /// <summary>
    /// Whether to override existing human-verified or rejected states.
    /// When true, automation checks will run even on entities that have been manually verified or rejected.
    /// </summary>
    public bool OverrideVerifiedState { get; init; }
}
