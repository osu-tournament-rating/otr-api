using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// A beatmapset with beatmaps included
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class BeatmapsetDTO : BeatmapsetCompactDTO
{
    /// <summary>
    /// Beatmaps which are part of this set
    /// </summary>
    public ICollection<BeatmapDTO> Beatmaps { get; set; } = [];
}
