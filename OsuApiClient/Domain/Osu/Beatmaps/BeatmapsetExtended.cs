using OsuApiClient.Domain.Osu.Users;

namespace OsuApiClient.Domain.Osu.Beatmaps;

/// <summary>
/// Represents a <see cref="BeatmapSet"/> with additional attributes
/// </summary>
public class BeatmapsetExtended : Beatmapset
{
    /// <summary>
    /// All beatmaps in the set
    /// </summary>
    public BeatmapExtended[] Beatmaps { get; init; } = [];

    /// <summary>
    /// A list of user data for any beatmap owners or nominators
    /// </summary>
    public User[] RelatedUsers { get; init; } = [];

    /// <summary>
    /// The user that owns the set
    /// </summary>
    public User? User { get; init; }
}
