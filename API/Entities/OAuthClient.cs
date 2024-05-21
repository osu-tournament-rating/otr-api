using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Interfaces;

namespace API.Entities;

[Table("oauth_clients")]
public class OAuthClient : IUpdateableEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [MaxLength(128)]
    [Column("secret")]
    public string Secret { get; set; } = string.Empty;

    [Column("scopes")]
    public string[] Scopes { get; set; } = [];

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("created")]
    public DateTime Created { get; set; }

    [Column("updated")]
    public DateTime? Updated { get; set; }

    // Column name and value initialization is handled via OtrContext
    /// <summary>
    /// Represents values that override the API rate limit for the OAuthClient
    /// </summary>
    public RateLimitOverrides RateLimitOverrides { get; set; } = null!;

    [InverseProperty("Clients")]
    public User User { get; set; } = null!;
}
