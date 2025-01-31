namespace API.DTOs;

/// <summary>
/// Represents a beatmap
/// </summary>
public class BeatmapDTO
{
    /// <summary>
    /// Id of the beatmap
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// osu! id of the beatmap
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// Beats per minute
    /// </summary>
    public double? Bpm { get; set; }

    /// <summary>
    /// Star rating
    /// </summary>
    public double Sr { get; set; }

    /// <summary>
    /// Circle size
    /// </summary>
    public double Cs { get; set; }

    /// <summary>
    /// Approach rate
    /// </summary>
    public double Ar { get; set; }

    /// <summary>
    /// HP drain rate
    /// </summary>
    public double Hp { get; set; }

    /// <summary>
    /// Overall difficulty
    /// </summary>
    public double Od { get; set; }

    /// <summary>
    /// Song length
    /// </summary>
    public long TotalLength { get; set; }

    /// <summary>
    /// Name of the difficulty
    /// </summary>
    public string? DiffName { get; set; }

    /// <summary>
    /// Beatmapset id
    /// </summary>
    public int? BeatmapSetId { get; set; }

    /// <summary>
    /// Beatmapset
    /// </summary>
    public BeatmapSetCompactDTO? BeatmapSet { get; init; }

    /// <summary>
    /// Beatmap attributes
    /// </summary>
    public ICollection<BeatmapAttributesDTO> Attributes { get; set; } = [];

    /// <summary>
    /// Beatmap creators
    /// </summary>
    public ICollection<PlayerCompactDTO> Creators { get; set; } = [];
}
