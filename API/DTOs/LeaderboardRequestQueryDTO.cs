using System.ComponentModel.DataAnnotations;
using API.Enums;
using API.ModelBinders;
using Database.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.DTOs;

public class LeaderboardRequestQueryDTO
{
    public Ruleset Ruleset { get; init; }
    /// <summary>
    /// The zero-indexed page offset. Page 0 returns the first PageSize results.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Page count out of bounds")]
    public int Page { get; set; }

    /// <summary>
    /// The number of elements to return per page
    /// </summary>
    [Range(5, 100, ErrorMessage = "The page size must be between 5 and 100")]
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// Defines whether the leaderboard should be global or filtered by country
    /// </summary>
    [Range(0, 1, ErrorMessage = "Invalid chart type provided. Must be 0 (global) or 1 (country)")]
    public LeaderboardChartType ChartType { get; init; } = LeaderboardChartType.Global;

    [ModelBinder(BinderType = typeof(LeaderboardFilterModelBinder))]
    // Note: When debugging in swagger, you'll want to clear
    // out the default filter object to return an unfiltered leaderboard.
    public LeaderboardFilterDTO Filter { get; init; } = new();
}
