using API.Utilities;
using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents rating tier progress data
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class TierProgressDTO(double rating)
{
    /// <summary>
    /// Current tier
    /// </summary>
    public string CurrentTier { get; set; } = RatingUtils.GetMajorTier(rating);

    /// <summary>
    /// Current sub tier
    /// </summary>
    public int? CurrentSubTier { get; set; } = RatingUtils.GetSubTier(rating);

    /// <summary>
    /// Name of the next major tier
    /// </summary>
    /// <remarks>
    /// Null if there is no next major tier, e.g. when the rating value is within the maximum tier
    /// </remarks>
    public string? NextTier { get; set; } = RatingUtils.GetNextTierRating(rating).HasValue ? RatingUtils.GetMajorTier(RatingUtils.GetNextTierRating(rating)!.Value) : null;

    /// <summary>
    /// Next sub tier
    /// </summary>
    public int? NextSubTier { get; set; } = RatingUtils.GetNextSubTier(rating);

    /// <summary>
    /// Rating required to reach next sub tier
    /// </summary>
    public double RatingForNextTier { get; set; } = RatingUtils.GetNextTierRatingDelta(rating);

    /// <summary>
    /// Rating required to reach next major tier
    /// </summary>
    public double RatingForNextMajorTier { get; set; } = RatingUtils.GetNextMajorTierRatingDelta(rating);

    /// <summary>
    /// Major tier following current major tier
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
