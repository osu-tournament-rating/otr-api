using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Database.Entities;

/// <summary>
/// A match played in a <see cref="Tournament"/>
/// </summary>
[Table("matches")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Match : MatchEntityBase
{
    /// <summary>
    /// The <see cref="Tournament"/> that the match was played in
    /// </summary>
    public Tournament Tournament { get; set; } = null!;

    /// <summary>
    /// The <see cref="User"/> that submitted the match
    /// </summary>
    public User? SubmittedByUser { get; set; }

    /// <summary>
    /// The <see cref="User"/> that verified the match
    /// </summary>
    public User? VerifiedByUser { get; set; }

    /// <summary>
    /// The <see cref="MatchWinRecord"/> for the match
    /// </summary>
    public MatchWinRecord? WinRecord { get; set; }

    /// <summary>
    /// A collection of the <see cref="Game"/>s played in the match
    /// </summary>
    public ICollection<Game> Games { get; set; } = new List<Game>();

    /// <summary>
    /// A collection of <see cref="Processor.MatchRatingStats"/> for the match
    /// </summary>
    public ICollection<MatchRatingStats> MatchRatingStats { get; set; } = new List<MatchRatingStats>();

    /// <summary>
    /// A collection of <see cref="Processor.PlayerMatchStats"/> for the match
    /// </summary>
    public ICollection<PlayerMatchStats> PlayerMatchStats { get; set; } = new List<PlayerMatchStats>();
}
