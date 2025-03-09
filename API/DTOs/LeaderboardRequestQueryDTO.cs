using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using API.DTOs.Interfaces;
using Common.Enums;
using Common.Enums.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.DTOs;

/// <summary>
/// Filtering parameters for leaderboard requests
/// </summary>
public class LeaderboardRequestQueryDTO : IPaginated
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
    [EnumDataType(typeof(Ruleset))]
    public Ruleset Ruleset { get; init; } = Ruleset.Osu;

    /// <summary>
    /// Defines whether the leaderboard should be global or filtered by country
    /// </summary>
    [FromQuery]
    [DefaultValue(LeaderboardChartType.Global)]
    [EnumDataType(typeof(LeaderboardChartType))]
    public LeaderboardChartType ChartType { get; init; } = LeaderboardChartType.Global;

    /// <summary>
    /// An optional country code to filter by
    /// </summary>
    /// <remarks>ChartType must be set to Country for this to apply</remarks>
    [FromQuery]
    [DefaultValue(null)]
    public string? Country { get; set; }

    [BindNever]
    public LeaderboardFilterDTO Filter { get; set; } = new();
}
