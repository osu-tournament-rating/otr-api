using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Interfaces;

namespace Database.Entities;

/// <summary>
/// An OAuth2 Client for the o!TR API
/// </summary>
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
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// A collection of string literals denoting special permissions granted to the client
    /// </summary>
    public string[] Scopes { get; set; } = [];

    /// <summary>
    /// Value that overrides the default API rate limit for the client
    /// </summary>
    public int? RateLimitOverride { get; set; }

    /// <summary>
    /// Id of the <see cref="Entities.User"/> that owns the <see cref="OAuthClient"/>
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The <see cref="Entities.User"/> that owns the <see cref="OAuthClient"/>
    /// </summary>
    public User User { get; set; } = null!;

    public ICollection<OAuthClientAdminNote> AdminNotes { get; set; } = [];
}
