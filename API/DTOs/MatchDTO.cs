using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using Database.Enums.Verification;

namespace API.DTOs;

/// <summary>
/// Represents a played match
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class MatchDTO : MatchCompactDTO
{
    /// <summary>
    /// The <see cref="TournamentCompactDTO"/> this match was played in
    /// </summary>
    public TournamentCompactDTO Tournament { get; set; } = null!;

    /// <summary>
    /// The participating <see cref="Player"/>s
    /// </summary>
    public ICollection<PlayerCompactDTO> Players { get; set; } = [];

    /// <summary>
    /// List of games played during the match
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<GameDTO> Games { get; set; } = [];

    /// <summary>
    /// All associated admin notes
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public ICollection<AdminNoteDTO> AdminNotes { get; init; } = [];
}
