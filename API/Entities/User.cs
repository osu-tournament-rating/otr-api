using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("users")]
public class User
{
	[Key]
	[Column("id")]
	public int Id { get; set; }
	[Column("player_id")]
	public int? PlayerId { get; set; }
	[Column("last_login", TypeName = "timestamp with time zone")]
	public DateTime? LastLogin { get; set; }
	[Column("created", TypeName = "timestamp with time zone")]
	public DateTime Created { get; set; }
	/// <summary>
	/// Comma-delimited list of roles (e.g. user, admin, etc.)
	/// </summary>
	[Column("roles")]
	public string[] Roles { get; set; } = Array.Empty<string>();
	[Column("updated", TypeName = "timestamp with time zone")]
	public DateTime? Updated { get; set; }
	[ForeignKey("PlayerId")]
	[InverseProperty("User")]
	public virtual Player Player { get; set; } = null!;
	public IEnumerable<Match>? SubmittedMatches { get; set; }
	// Assuming the user has permission to verify, the matches they do verify will be here
	public virtual ICollection<Match>? VerifiedMatches { get; set; }
	[InverseProperty("Verifier")]
	public virtual ICollection<MatchDuplicate>? VerifiedDuplicates { get; set; }
}