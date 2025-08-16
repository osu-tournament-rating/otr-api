using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Common.Enums;

namespace Database.Entities;

/// <summary>
/// Core beatmap information
/// </summary>
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class Beatmap : UpdateableEntityBase
{
    /// <summary>
    /// osu! beatmap ID
    /// </summary>
    public long OsuId { get; init; }

    /// <summary>
    /// Status of data fetching from the osu! API
    /// </summary>
    public DataFetchStatus DataFetchStatus { get; set; }

    /// <summary>
    /// Ruleset
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// Ranked status
    /// </summary>
    public BeatmapRankedStatus RankedStatus { get; set; }

    /// <summary>
    /// Difficulty name
    /// </summary>
    [MaxLength(512)]
    public string DiffName { get; set; } = string.Empty;

    /// <summary>
    /// Total length in seconds
    /// </summary>
    public long TotalLength { get; set; }

    /// <summary>
    /// Drain length in seconds
    /// </summary>
    public int DrainLength { get; set; }

    /// <summary>
    /// Beats per minute
    /// </summary>
    public double Bpm { get; set; }

    /// <summary>
    /// Number of circles in the beatmap
    /// </summary>
    public int CountCircle { get; set; }

    /// <summary>
    /// Number of sliders in the beatmap
    /// </summary>
    public int CountSlider { get; set; }

    /// <summary>
    /// Number of spinners in the beatmap
    /// </summary>
    public int CountSpinner { get; set; }

    /// <summary>
    /// Circle size
    /// </summary>
    public double Cs { get; set; }

    /// <summary>
    /// HP drain rate
    /// </summary>
    public double Hp { get; set; }

    /// <summary>
    /// Overall difficulty
    /// </summary>
    public double Od { get; set; }

    /// <summary>
    /// Approach rate
    /// </summary>
    public double Ar { get; set; }

    /// <summary>
    /// Star rating (No Mod)
    /// </summary>
    public double Sr { get; set; }

    /// <summary>
    /// Maximum combo, if available
    /// </summary>
    public int? MaxCombo { get; set; }

    /// <summary>
    /// Id of the associated beatmapset
    /// </summary>
    public int? BeatmapsetId { get; init; }

    /// <summary>
    /// The associated beatmapset, if available
    /// </summary>
    public Beatmapset? Beatmapset { get; set; }

    /// <summary>
    /// Collection of players who created this beatmap
    /// </summary>
    public ICollection<Player> Creators { get; init; } = [];

    /// <summary>
    /// Collection of games played on this beatmap
    /// </summary>
    public ICollection<Game> Games { get; init; } = [];

    /// <summary>
    /// Collection of tournaments this beatmap is pooled in
    /// </summary>
    public ICollection<Tournament> TournamentsPooledIn { get; init; } = [];

    /// <summary>
    /// Collection of attributes for this beatmap
    /// </summary>
    public ICollection<BeatmapAttributes> Attributes { get; init; } = [];
}
