using System.ComponentModel.DataAnnotations;
using Database.Enums;

namespace API.DTOs;

/// <summary>
/// Represents a compact version of a beatmapset
/// </summary>
public class BeatmapSetCompactDTO
{
    /// <summary>
    /// Beatmapset id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// osu! beatmapset id
    /// </summary>
    public long OsuId { get; set; }

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
    /// Id of the Player who created the set
    /// </summary>
    public int CreatorId { get; set; }

    /// <summary>
    /// The set creator
    /// </summary>
    public PlayerCompactDTO? Creator { get; set; }
}
