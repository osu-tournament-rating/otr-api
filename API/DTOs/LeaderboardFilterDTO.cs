using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.DTOs;

/// <summary>
/// Filters for the leaderboard
/// </summary>
public class LeaderboardFilterDTO
{
    /// <summary>
    /// Rank floor
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int? MinRank { get; init; }

    /// <summary>
    /// Rank ceiling
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int? MaxRank { get; init; }

    /// <summary>
    /// Rating floor
    /// </summary>
    [Range(100, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int? MinRating { get; init; }

    /// <summary>
    /// Rating ceiling
    /// </summary>
    [Range(100, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int? MaxRating { get; init; }

    /// <summary>
    /// Minimum Maximum number of matches played
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int? MinMatches { get; init; }

    /// <summary>
    /// Maximum number of matches played
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int? MaxMatches { get; init; }

    /// <summary>
    /// Minimum win rate
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public double? MinWinRate { get; init; }

    /// <summary>
    /// Maximum win rate
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public double? MaxWinRate { get; init; }

    /// <summary>
    /// A collection of optional filters for tiers
    /// </summary>
    [BindNever]
    [JsonIgnore]
    public LeaderboardTierFilterDTO? TierFilters { get; set; } = new();
}
