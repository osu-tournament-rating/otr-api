using System.ComponentModel.DataAnnotations;
using Common.Enums;
using JetBrains.Annotations;

namespace Database.Entities;

/// <summary>
/// Represents a beatmapset in the osu! API.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class Beatmapset : UpdateableEntityBase
{
    /// <summary>
    /// osu! beatmapset ID
    /// </summary>
    public long OsuId { get; init; }

    /// <summary>
    /// Id of the Player who created the set
    /// </summary>
    public int? CreatorId { get; init; }

    /// <summary>
    /// Artist
    /// </summary>
    [MaxLength(512)]
    public string Artist { get; init; } = string.Empty;

    /// <summary>
    /// Title
    /// </summary>
    [MaxLength(512)]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Ranked status
    /// </summary>
    public BeatmapRankedStatus RankedStatus { get; init; }

    /// <summary>
    /// Date of ranking, if applicable
    /// </summary>
    public DateTime? RankedDate { get; init; }

    /// <summary>
    /// Date of submission
    /// </summary>
    public DateTime? SubmittedDate { get; init; }

    /// <summary>
    /// The set creator
    /// </summary>
    public Player? Creator { get; set; }

    /// <summary>
    /// Collection of beatmaps in this beatmapset
    /// </summary>
    public ICollection<Beatmap> Beatmaps { get; init; } = [];
}
