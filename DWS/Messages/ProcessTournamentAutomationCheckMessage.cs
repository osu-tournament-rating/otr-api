using System.ComponentModel.DataAnnotations;

namespace DWS.Messages;

/// <summary>
/// Message used to request processing automation checks for a tournament.
/// </summary>
public record ProcessTournamentAutomationCheckMessage : BaseMessage
{
    /// <summary>
    /// The tournament ID to process automation checks for.
    /// </summary>
    [Required]
    public int TournamentId { get; init; }
}
