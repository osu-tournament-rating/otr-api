using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using API.DTOs.Interfaces;
using Common.Enums;
using API.Utilities;
using API.Utilities.DataAnnotations;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace API.DTOs;

/// <summary>
/// Filtering parameters for leaderboard requests
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class LeaderboardRequestQueryDTO : IPaginated, IValidatableObject
{
    [FromQuery]
    [Positive]
    [DefaultValue(1)]
    public int Page { get; init; } = 1;

    [FromQuery]
    [Range(10, 100)]
    [DefaultValue(25)]
    public int PageSize { get; init; } = 25;

    /// <summary>
    /// Ruleset for leaderboard data
    /// </summary>
    [FromQuery]
    [DefaultValue(Ruleset.Osu)]
    [EnumDataType(typeof(Ruleset))]
    public Ruleset Ruleset { get; init; } = Ruleset.Osu;

    /// <summary>
    /// ISO country code
    /// </summary>
    /// <remarks>Leaderboard will be global if not provided</remarks>
    [FromQuery]
    [DefaultValue(null)]
    public string? Country { get; init; }

    /// <summary>
    /// osu! rank floor
    /// </summary>
    /// <remarks>
    /// The "better" inclusive rank bound.
    /// If given, only players with a rank greater than or equal to this value will be included
    /// </remarks>
    [FromQuery]
    [Positive]
    public int? MinOsuRank { get; init; }

    /// <summary>
    /// osu! rank ceiling
    /// </summary>
    /// <remarks>
    /// The "worse" inclusive rank bound.
    /// If given, only players with a rank less than or equal to this value will be included
    /// </remarks>
    [FromQuery]
    [Positive]
    public int? MaxOsuRank { get; init; }

    /// <summary>
    /// Rating floor
    /// </summary>
    /// <remarks>
    /// The "worse" inclusive rating bound.
    /// If given, only players with a rating greater than or equal to this value will be included
    /// </remarks>
    [FromQuery]
    [Range(RatingUtils.RatingMinimum, int.MaxValue)]
    public int? MinRating { get; init; }

    /// <summary>
    /// Rating ceiling
    /// </summary>
    /// <remarks>
    /// The "better" inclusive rating bound.
    /// If given, only players with a rating less than or equal to this value will be included
    /// </remarks>
    [FromQuery]
    [Range(RatingUtils.RatingMinimum, int.MaxValue)]
    public int? MaxRating { get; init; }

    /// <summary>
    /// Minimum number of matches played
    /// </summary>
    [FromQuery]
    [Positive]
    public int? MinMatches { get; init; }

    /// <summary>
    /// Maximum number of matches played
    /// </summary>
    [FromQuery]
    [Positive]
    public int? MaxMatches { get; init; }

    /// <summary>
    /// Minimum win rate
    /// </summary>
    [FromQuery]
    [Percentage]
    public double? MinWinRate { get; init; }

    /// <summary>
    /// Maximum win rate
    /// </summary>
    [FromQuery]
    [Percentage]
    public double? MaxWinRate { get; init; }

    /*
     * A collection of booleans representing which tiers to filter on the leaderboard.
     *
     * False = Default, no behavioral change
     * True = Explicitly included in leaderboard results
     *
     * If *all* tiers are set to false, or all tiers are set to true, the leaderboard will return
     * as if no tier filters were applied.
     *
     * For example, if Bronze and Emerald are true and everything else is false,
     * then only Bronze and Emerald players will show up in the leaderboard
     * (specifically, Bronze III-I and Emerald III-I)
    */

    /// <summary>
    /// Explicitly include bronze players
    /// </summary>
    [FromQuery]
    public bool Bronze { get; init; }

    /// <summary>
    /// Explicitly include silver players
    /// </summary>
    [FromQuery]
    public bool Silver { get; init; }

    /// <summary>
    /// Explicitly include gold players
    /// </summary>
    [FromQuery]
    public bool Gold { get; init; }

    /// <summary>
    /// Explicitly include platinum players
    /// </summary>
    [FromQuery]
    public bool Platinum { get; init; }

    /// <summary>
    /// Explicitly include emerald players
    /// </summary>
    [FromQuery]
    public bool Emerald { get; init; }

    /// <summary>
    /// Explicitly include diamond players
    /// </summary>
    [FromQuery]
    public bool Diamond { get; init; }

    /// <summary>
    /// Explicitly include master players
    /// </summary>
    [FromQuery]
    public bool Master { get; init; }

    /// <summary>
    /// Explicitly include grandmaster players
    /// </summary>
    [FromQuery]
    public bool Grandmaster { get; init; }

    /// <summary>
    /// Explicitly include elite grandmaster players
    /// </summary>
    [FromQuery]
    public bool EliteGrandmaster { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinOsuRank > MaxOsuRank)
        {
            yield return new ValidationResult(
                $"Value for {nameof(MinOsuRank)} must be less than or equal to {nameof(MaxOsuRank)}",
                [nameof(MinOsuRank), nameof(MaxOsuRank)]
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
