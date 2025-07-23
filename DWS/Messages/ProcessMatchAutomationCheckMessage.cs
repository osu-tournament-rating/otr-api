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
}
