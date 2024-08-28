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
    /// Artist of the song
    /// </summary>
    public string Artist { get; set; } = null!;

    /// <summary>
    /// osu! id of the beatmap
    /// </summary>
    public long OsuId { get; set; }

    /// <summary>
    /// Beats per minute
    /// </summary>
    public double? Bpm { get; set; }

    /// <summary>
    /// osu! id of the mapper
    /// </summary>
    public long MapperId { get; set; }

    /// <summary>
    /// osu! username of the mapper
    /// </summary>
    public string MapperName { get; set; } = null!;

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
    /// Hp
    /// </summary>
    public double Hp { get; set; }

    /// <summary>
    /// Overall difficulty
    /// </summary>
    public double Od { get; set; }

    /// <summary>
    /// Song length
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// Title of the beatmap / song
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Name of the difficulty
    /// </summary>
    public string? DiffName { get; set; }
}
