using API.Utilities;

namespace API.DTOs;

/// <summary>
/// Represents a search result for a player for a given ruleset
/// </summary>
public class PlayerSearchResultDTO
{
    /// <summary>
    /// Id of the player
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// osu! id of the player
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// Rating of the player for the given ruleset
    /// </summary>
    public double? Rating { get; set; }

    /// <summary>
    /// Current global rank of the player for the given ruleset
    /// </summary>
    public int? GlobalRank { get; set; }

    /// <summary>
    /// Current rating tier of the player for the given ruleset
    /// </summary>
    public string? RatingTier => Rating.HasValue ? RatingUtils.GetMajorTier(Rating.Value) : null;

    /// <summary>
    /// osu! username of the player
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Link to an osu! thumbnail for the player
    /// </summary>
    public required string Thumbnail { get; set; }

    /// <summary>
    /// Denotes the player is a friend of the requesting user
    /// </summary>
    public bool IsFriend;
}
