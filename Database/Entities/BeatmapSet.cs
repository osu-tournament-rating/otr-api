using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Enums;
using JetBrains.Annotations;

namespace Database.Entities;

/// <summary>
/// Represents a beatmapset in the osu! API.
/// </summary>
[Table("beatmapsets")]
[UsedImplicitly]
public class BeatmapSet : UpdateableEntityBase
{
    /// <summary>
    /// osu! beatmapset ID
    /// </summary>
    [Column("osu_id")]
    public long OsuId { get; set; }

    /// <summary>
    /// Id of the Player who created the set
    /// </summary>
    [Column("creator_id")]
    public int CreatorId { get; set; }

    /// <summary>
    /// Artist
    /// </summary>
    [Column("artist")]
    [MaxLength(512)]
    public string Artist { get; set; } = string.Empty;

    /// <summary>
    /// Title
    /// </summary>
    [Column("title")]
    [MaxLength(512)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Ranked status
    /// </summary>
    [Column("ranked_status")]
    public BeatmapRankedStatus RankedStatus { get; set; }

    /// <summary>
    /// Date of ranking, if applicable
    /// </summary>
    [Column("ranked_date")]
    public DateTime? RankedDate { get; set; }

    /// <summary>
    /// Date of submission
    /// </summary>
    [Column("submitted_date")]
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
