using API.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("matches")]
[Index("MatchId", Name = "osumatches_matchid", IsUnique = true)]
public class Match
{
	[Key]
	[Column("id")]
	public int Id { get; set; }
	[Column("match_id")]
	public long MatchId { get; set; }
	[Column("name")]
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
	public string? VerificationInfo { get; set; }
	[Column("verification_source")]
	public int? VerificationSource { get; set; }
	[Column("verification_status")]
	public int? VerificationStatus { get; set; }
	[InverseProperty("Match")]
	public virtual ICollection<Game> Games { get; set; } = new List<Game>();
	[InverseProperty("Match")]
	public virtual ICollection<RatingHistory> RatingHistories { get; set; } = new List<RatingHistory>();
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
	public VerificationStatus? VerificationStatusEnum
	{
		get
		{
			if (VerificationStatus != null)
			{
				return (VerificationStatus)VerificationStatus;
			}

			return null;
		}
	}
}