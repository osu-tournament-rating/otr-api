using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using API.DTOs.Interfaces;
using API.Enums;
using Database.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.DTOs;

/// <summary>
/// Filtering parameters for leaderboard requests
/// </summary>
public class LeaderboardRequestQueryDTO : IPaginatedRequestQueryDTO
{
    [FromQuery]
    [Range(1, int.MaxValue)]
    [DefaultValue(1)]
    public int Page { get; init; } = 1;

    [FromQuery]
    [Range(10, 100)]
    [DefaultValue(50)]
    public int PageSize { get; init; } = 50;

    /// <summary>
    /// Ruleset for leaderboard data
    /// </summary>
    [FromQuery]
    [DefaultValue(Ruleset.Osu)]
    public Ruleset Ruleset { get; init; } = Ruleset.Osu;

    /// <summary>
    /// Defines whether the leaderboard should be global or filtered by country
    /// </summary>
    [FromQuery]
    [DefaultValue(LeaderboardChartType.Global)]
    public LeaderboardChartType ChartType { get; init; } = LeaderboardChartType.Global;

    [BindNever]
    public LeaderboardFilterDTO Filter { get; set; } = new();
}
