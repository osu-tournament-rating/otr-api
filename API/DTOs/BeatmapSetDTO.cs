namespace API.DTOs;

public class BeatmapSetDTO : BeatmapSetCompactDTO
{
    public ICollection<BeatmapDTO> Beatmaps { get; set; } = [];
}
