using System.ComponentModel.DataAnnotations;

namespace DWS.Messages;

/// <summary>
/// Message used to request processing automation checks for a game.
/// </summary>
public record ProcessGameAutomationCheckMessage : BaseMessage
{
    /// <summary>
    /// The game ID to process automation checks for.
    /// </summary>
    [Required]
    public int GameId { get; init; }
}
