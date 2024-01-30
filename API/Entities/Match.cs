using API.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("matches")]
[Index("MatchId", Name = "osumatches_matchid", IsUnique = true)]
public class Match
{
	[Key]
	[Column("id")]
	public int Id { get; set; }
	/// <summary>
	///  The osu! match id
	/// </summary>
	[Column("match_id")]
	public long MatchId { get; set; }
	[Column("name")]
	[MaxLength(512)]
	public string? Name { get; set; }
	[Column("start_time", TypeName = "timestamp with time zone")]
	public DateTime? StartTime { get; set; }
	[Column("created", TypeName = "timestamp with time zone")]
	public DateTime Created { get; set; }
	[Column("updated", TypeName = "timestamp with time zone")]
	public DateTime? Updated { get; set; }
	[Column("end_time", TypeName = "timestamp with time zone")]
	public DateTime? EndTime { get; set; }
	[Column("verification_info")]
	[MaxLength(512)]
	public string? VerificationInfo { get; set; }
	[Column("verification_source")]
	public int? VerificationSource { get; set; }
	[Column("verification_status")]
	public int? VerificationStatus { get; set; }
	[Column("submitted_by_user")]
	public int? SubmitterUserId { get; set; }
	[Column("verified_by_user")]
	public int? VerifierUserId { get; set; }
	[Column("tournament_id")]
	public int TournamentId { get; set; }
	/// <summary>
	/// Checked by a background worker to see if the match needs to be processed.
	/// </summary>
	[Column("needs_auto_check")]
	public bool? NeedsAutoCheck { get; set; }
	/// <summary>
	/// A flag indicating whether this match needs to be processed by the API. 
	/// </summary>
	[Column("is_api_processed")]
	public bool? IsApiProcessed { get; set; }
	public User? SubmittedBy { get; set; }
	public User? VerifiedBy { get; set; }
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