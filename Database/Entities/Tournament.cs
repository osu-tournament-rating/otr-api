using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Database.Entities;

/// <summary>
/// An osu! tournament
/// </summary>
[Table("tournaments")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Tournament : TournamentEntityBase
{
    /// <summary>
    /// The <see cref="User"/> that submitted the tournament
    /// </summary>
    public User? SubmittedByUser { get; set; }

    /// <summary>
    /// The <see cref="User"/> that verified the tournament
    /// </summary>
    public User? VerifiedByUser { get; set; }

    /// <summary>
    /// A collection of <see cref="Match"/>es played in the tournament
    /// </summary>
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
