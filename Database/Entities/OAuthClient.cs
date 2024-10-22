using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;

namespace Database.Entities;

/// <summary>
/// An OAuth2 Client for the o!TR API
/// </summary>
[Table("oauth_clients")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class OAuthClient : UpdateableEntityBase, IAdminNotableEntity<OAuthClientAdminNote>
{
    /// <summary>
    /// Authorization secret
    /// </summary>
    /// <remarks>
    /// Client secrets are stored as hashes and only returned as plaintext when generated for the first time
    /// </remarks>
    [MaxLength(128)]
    [Column("secret")]
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// A collection of string literals denoting special permissions granted to the client
    /// </summary>
    [Column("scopes")]
    public string[] Scopes { get; set; } = [];

    // NOTE: Column name and value initialization is handled via OtrContext
    /// <summary>
    /// Values that override the default API rate limit configuration for the client
    /// </summary>
    public RateLimitOverrides RateLimitOverrides { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Entities.User"/> that owns the <see cref="OAuthClient"/>
    /// </summary>
    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// The <see cref="Entities.User"/> that owns the <see cref="OAuthClient"/>
    /// </summary>
    public User User { get; set; } = null!;

    public ICollection<OAuthClientAdminNote> AdminNotes { get; set; } = new List<OAuthClientAdminNote>();
}
