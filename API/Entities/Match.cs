using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using API.Entities.Interfaces;
using API.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Entities;

/// <summary>
/// Represents a tournament match
/// </summary>
[Table("matches")]
// ReSharper disable once StringLiteralTypo
[Index("MatchId", Name = "osumatches_matchid", IsUnique = true)]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class Match : MatchEntityBase, IUpdateableEntity
{
    /// <summary>
    /// Date the entity was created
    /// </summary>
    [Column("created", TypeName = "timestamp with time zone")]
    public DateTime Created { get; set; }

    /// <summary>
    /// Date of the last update to the entity
    /// </summary>
    [Column("updated", TypeName = "timestamp with time zone")]
    public DateTime? Updated { get; set; }

    /// <summary>
    /// The user that submitted the match
    /// </summary>
    [InverseProperty("SubmittedMatches")]
    public virtual User? SubmittedBy { get; set; }

    /// <summary>
    /// The user that verified the match
    /// </summary>
    [InverseProperty("VerifiedMatches")]
    public virtual User? VerifiedBy { get; set; }

    /// <summary>
    /// All games played during the match
    /// </summary>
    [InverseProperty("Match")]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    /// <summary>
    /// The tournament that the match was played in
    /// </summary>
    [InverseProperty("Matches")]
    public virtual Tournament Tournament { get; set; } = null!;

    /// <summary>
    /// All player stats for the match
    /// </summary>
    [InverseProperty("Match")]
    public virtual ICollection<PlayerMatchStats> Stats { get; set; } = new List<PlayerMatchStats>();

    /// <summary>
    /// All rating stats for the match
    /// </summary>
    [InverseProperty("Match")]
    public virtual ICollection<MatchRatingStats> RatingStats { get; set; } = new List<MatchRatingStats>();

    /// <summary>
    /// The win record for the match
    /// </summary>
    [InverseProperty("Match")]
    public virtual MatchWinRecord WinRecord { get; set; } = new();

    [NotMapped]
    public MatchVerificationSource? VerificationSourceEnum
    {
        get
        {
            if (VerificationSource != null)
            {
                return (MatchVerificationSource)VerificationSource;
            }

            return null;
        }
    }

    [NotMapped]
    public MatchVerificationStatus? VerificationStatusEnum
    {
        get
        {
            if (VerificationStatus != null)
            {
                return (MatchVerificationStatus)VerificationStatus;
            }

            return null;
        }
    }
}
