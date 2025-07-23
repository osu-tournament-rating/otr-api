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
}
