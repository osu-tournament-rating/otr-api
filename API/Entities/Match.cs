using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Interfaces;
using API.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Entities;

[Table("matches")]
[Index("MatchId", Name = "osumatches_matchid", IsUnique = true)]
public class Match : MatchEntityBase, IEntityBase
{
    [Column("created", TypeName = "timestamp with time zone")]
    public DateTime Created { get; set; }

    [Column("updated", TypeName = "timestamp with time zone")]
    public DateTime? Updated { get; set; }
    public virtual User? SubmittedBy { get; set; }
    public virtual User? VerifiedBy { get; set; }

    [InverseProperty("Match")]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    [InverseProperty("Matches")]
    public virtual Tournament Tournament { get; set; } = null!;

    [InverseProperty("Match")]
    public virtual ICollection<PlayerMatchStats> Stats { get; set; } = new List<PlayerMatchStats>();

    [InverseProperty("Match")]
    public virtual ICollection<MatchRatingStats> RatingStats { get; set; } = new List<MatchRatingStats>();

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
