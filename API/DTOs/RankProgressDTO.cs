namespace API.DTOs;

/// <summary>
/// Represents rating tier progress data
/// </summary>
public class RankProgressDTO
{
    /// <summary>
    /// Current tier
    /// </summary>
    public string CurrentTier { get; set; } = null!;

    /// <summary>
    /// Current sub tier
    /// </summary>
    public int? CurrentSubTier { get; set; }

    /// <summary>
    /// Rating required to reach next sub tier
    /// </summary>
    public double RatingForNextTier { get; set; }

    /// <summary>
    /// Rating required to reach next major tier
    /// </summary>
    public double RatingForNextMajorTier { get; set; }

    /// <summary>
    /// Next major tier following current tier
    /// </summary>
    public string? NextMajorTier { get; set; }

    /// <summary>
    /// Progress to the next sub tier as a percentage
    /// </summary>
    public double? SubTierFillPercentage { get; set; }

    /// <summary>
    /// Progress to the next major tier as a percentage
    /// </summary>
    public double? MajorTierFillPercentage { get; set; }
}
