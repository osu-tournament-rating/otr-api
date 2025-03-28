using System.ComponentModel.DataAnnotations;
using Common.Enums;
using JetBrains.Annotations;

namespace Database.Entities;

/// <summary>
/// Represents a beatmapset in the osu! API.
/// </summary>
[UsedImplicitly]
public class Beatmapset : UpdateableEntityBase
{
    /// <summary>
    /// osu! beatmapset ID
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// Id of the Player who created the set
    /// </summary>
    public int? CreatorId { get; set; }

    /// <summary>
    /// Artist
    /// </summary>
    [MaxLength(512)]
    public string Artist { get; set; } = string.Empty;

    /// <summary>
    /// Title
    /// </summary>
    [MaxLength(512)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Ranked status
    /// </summary>
    public BeatmapRankedStatus RankedStatus { get; set; }

    /// <summary>
    /// Date of ranking, if applicable
    /// </summary>
    public DateTime? RankedDate { get; set; }

    /// <summary>
    /// Date of submission
    /// </summary>
    public DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// The set creator
    /// </summary>
    public Player? Creator { get; set; }

    /// <summary>
    /// Collection of beatmaps in this beatmapset
    /// </summary>
    public ICollection<Beatmap> Beatmaps { get; set; } = [];
}
