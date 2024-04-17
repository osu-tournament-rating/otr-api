using API.Osu;
using API.Osu.Enums;

namespace API.DTOs;

/// <summary>
/// Represents user account information
/// </summary>
public class UserDTO
{
    /// <summary>
    /// Id of the user
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// List of permissions granted to the user
    /// </summary>
    public string[]? Scopes { get; set; }

    /// <summary>
    /// Id of the associated player
    /// </summary>
    public int? PlayerId { get; set; }

    /// <summary>
    /// osu! id of the associated player
    /// </summary>
    public long? OsuId { get; set; }

    /// <summary>
    /// osu! country country code of the associated player
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// osu! username of the associated player
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Preferred ruleset of the associated player
    /// </summary>
    public Ruleset? Ruleset { get; set; }
}
