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
    /// Comma-delimited list of scopes
    /// </summary>
    [Column("scopes")]
    public string[] Scopes { get; set; } = [];

    [Column("updated", TypeName = "timestamp with time zone")]
    public DateTime? Updated { get; set; }

    // // Column name and value initialization is handled via OtrContext
    /// <summary>
    /// Represents values that override the API rate limit for the User
    /// </summary>
    public RateLimitOverrides RateLimitOverrides { get; set; } = null!;

    [ForeignKey("PlayerId")]
    [InverseProperty("User")]
    public virtual Player Player { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<OAuthClient>? Clients { get; set; }
    public virtual ICollection<Match>? SubmittedMatches { get; set; }
    public virtual ICollection<Tournament>? SubmittedTournaments { get; set; }

    // Assuming the user has permission to verify, the matches they do verify will be here
    public virtual ICollection<Match>? VerifiedMatches { get; set; }

    [InverseProperty("Verifier")]
    public virtual ICollection<MatchDuplicate>? VerifiedDuplicates { get; set; }
}
