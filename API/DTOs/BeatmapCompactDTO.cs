using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents essential beatmap information without nested data
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class BeatmapCompactDTO
{
    /// <summary>
    /// Id of the beatmap
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// osu! id of the beatmap
    /// </summary>
    public long OsuId { get; init; }

    /// <summary>
    /// Name of the difficulty
    /// </summary>
    public string? DiffName { get; init; }

    /// <summary>
    /// Star rating
    /// </summary>
    public double Sr { get; init; }

    /// <summary>
    /// Beatmapset
    /// </summary>
    public BeatmapsetCompactDTO? Beatmapset { get; init; }
}
