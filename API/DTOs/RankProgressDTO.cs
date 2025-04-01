using API.Utilities;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents rating tier progress data
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class RankProgressDTO(double rating)
{
    /// <summary>
    /// Current sub tier
    /// </summary>
    public int? CurrentSubTier { get; set; } = RatingUtils.GetSubTier(rating);

    /// <summary>
    /// Rating required to reach next sub tier
    /// </summary>
    public double RatingForNextTier { get; set; } = RatingUtils.GetNextTierRatingDelta(rating);

    /// <summary>
    /// Rating required to reach next major tier
    /// </summary>
    public double RatingForNextMajorTier { get; set; } = RatingUtils.GetNextMajorTierRatingDelta(rating);

    /// <summary>
    /// Next major tier following current tier
    /// </summary>
    public string? NextMajorTier { get; set; } = RatingUtils.GetNextMajorTier(rating);

    /// <summary>
    /// Progress to the next sub tier as a percentage
    /// </summary>
    public double? SubTierFillPercentage { get; set; } = RatingUtils.GetNextTierFillPercentage(rating);

    /// <summary>
    /// Progress to the next major tier as a percentage
    /// </summary>
    public double? MajorTierFillPercentage { get; set; } = RatingUtils.GetNextMajorTierFillPercentage(rating);
}
