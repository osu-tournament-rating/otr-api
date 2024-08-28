using API.Enums;
using API.ModelBinders;
using Database.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.DTOs;

public class LeaderboardRequestQueryDTO
{
    public Ruleset Ruleset { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; } = 50;
    public LeaderboardChartType ChartType { get; set; } = LeaderboardChartType.Global;

    [ModelBinder(BinderType = typeof(LeaderboardFilterModelBinder))]
    public LeaderboardFilterDTO Filter { get; set; } = new();
}
