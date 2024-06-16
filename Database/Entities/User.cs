using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Entities.Interfaces;

namespace Database.Entities;

[Table("users")]
public class User : IUpdateableEntity
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

    // Column name and value initialization is handled via OtrContext
    /// <summary>
    /// Represents values that override the API rate limit for the User
    /// </summary>
    public RateLimitOverrides RateLimitOverrides { get; set; } = null!;

    /// <summary>
    /// Settings that control behaviors on the o!TR website
    /// </summary>
    public UserSettings Settings { get; set; } = null!;

    [ForeignKey("PlayerId")]
    [InverseProperty("User")]
    public virtual Player Player { get; set; } = null!;

    /// <summary>
    /// A collection of <see cref="OAuthClient"/>s owned by the user
    /// </summary>
    public ICollection<OAuthClient> Clients { get; set; } = new List<OAuthClient>();

    /// <summary>
    /// A collection of <see cref="Match"/>es submitted by the user
    /// </summary>
    public ICollection<Match> SubmittedMatches { get; set; } = new List<Match>();

    /// <summary>
    /// A collection of <see cref="Tournament"/>s submitted by the user
    /// </summary>
    public ICollection<Tournament> SubmittedTournaments { get; set; } = new List<Tournament>();
}
