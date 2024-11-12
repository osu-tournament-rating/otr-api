using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using API.Enums;
using Database.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.DTOs;

/// <summary>
/// Filtering parameters for leaderboard requests
/// </summary>
public class LeaderboardRequestQueryDTO
{
    /// <summary>
    /// Ruleset for leaderboard data
    /// </summary>
    [Required]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// The zero-indexed page offset. Page 0 returns the first PageSize results.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Page count out of bounds")]
    public int Page { get; set; }

    /// <summary>
    /// The number of elements to return per page
    /// </summary>
    [DefaultValue(50)]
    [Range(5, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int PageSize { get; init; } = 50;

    /// <summary>
    /// Defines whether the leaderboard should be global or filtered by country
    /// </summary>
    public LeaderboardChartType ChartType { get; init; } = LeaderboardChartType.Global;

    [BindNever]
    [JsonIgnore]
    public LeaderboardFilterDTO Filter { get; set; } = new();
}
