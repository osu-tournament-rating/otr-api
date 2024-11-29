using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.DTOs;

/// <summary>
/// Filters for the leaderboard
/// </summary>
public class LeaderboardFilterDTO : IValidatableObject
{
    /// <summary>
    /// Rank floor
    /// </summary>
    /// <remarks>
    /// The "better" inclusive rank bound.
    /// If given, only players with a rank greater than or equal to this value will be included
    /// </remarks>
    [FromQuery]
    [Range(1, int.MaxValue)]
    public int? MinRank { get; init; }

    /// <summary>
    /// Rank ceiling
    /// </summary>
    /// <remarks>
    /// The "worse" inclusive rank bound.
    /// If given, only players with a rank less than or equal to this value will be included
    /// </remarks>
    [FromQuery]
    [Range(1, int.MaxValue)]
    public int? MaxRank { get; init; }

    /// <summary>
    /// Rating floor
    /// </summary>
    /// <remarks>
    /// The "worse" inclusive rating bound.
    /// If given, only players with a rating greater than or equal to this value will be included
    /// </remarks>
    [FromQuery]
    [Range(100, int.MaxValue)]
    public int? MinRating { get; init; }

    /// <summary>
    /// Rating ceiling
    /// </summary>
    /// <remarks>
    /// The "better" inclusive rating bound.
    /// If given, only players with a rating less than or equal to this value will be included
    /// </remarks>
    [FromQuery]
    [Range(100, int.MaxValue)]
    public int? MaxRating { get; init; }

    /// <summary>
    /// Minimum number of matches played
    /// </summary>
    [FromQuery]
    [Range(0, int.MaxValue)]
    public int? MinMatches { get; init; }

    /// <summary>
    /// Maximum number of matches played
    /// </summary>
    [FromQuery]
    [Range(0, int.MaxValue)]
    public int? MaxMatches { get; init; }

    /// <summary>
    /// Minimum win rate
    /// </summary>
    [FromQuery]
    [Range(0.00, 1.00)]
    public double? MinWinRate { get; init; }

    /// <summary>
    /// Maximum win rate
    /// </summary>
    [FromQuery]
    [Range(0.00, 1.00)]
    public double? MaxWinRate { get; init; }

    /// <summary>
    /// A collection of optional filters for tiers
    /// </summary>
    [BindNever]
    public LeaderboardTierFilterDTO? TierFilters { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinRank > MaxRank)
        {
            yield return new ValidationResult(
                $"Value for {nameof(MinRank)} must be less than or equal to {nameof(MaxRank)}",
                [nameof(MinRank), nameof(MaxRank)]
            );
        }

        if (MinRating > MaxRating)
        {
            yield return new ValidationResult(
                $"Value for {nameof(MinRating)} must be less than or equal to {nameof(MaxRating)}",
                [nameof(MinRating), nameof(MaxRating)]
            );
        }

        if (MinMatches > MaxMatches)
        {
            yield return new ValidationResult(
                $"Value for {nameof(MinMatches)} must be less than or equal to {nameof(MaxMatches)}",
                [nameof(MinMatches), nameof(MaxMatches)]
            );
        }

        if (MinWinRate > MaxWinRate)
        {
            yield return new ValidationResult(
                $"Value for {nameof(MinWinRate)} must be less than or equal to {nameof(MaxWinRate)}",
                [nameof(MinWinRate), nameof(MaxWinRate)]
            );
        }
    }
}
