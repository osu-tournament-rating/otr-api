namespace API.DTOs;

/// <summary>
/// A beatmapset with beatmaps included
/// </summary>
public class BeatmapSetDTO : BeatmapSetCompactDTO
{
    /// <summary>
    /// Beatmaps which are part of this set
    /// </summary>
    public ICollection<BeatmapDTO> Beatmaps { get; set; } = [];
}
