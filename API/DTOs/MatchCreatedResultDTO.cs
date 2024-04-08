using API.Controllers;
using API.DTOs.Interfaces;
using Newtonsoft.Json;

namespace API.DTOs;

/// <summary>
/// Represents a created match
/// </summary>
public class MatchCreatedResultDTO : CreatedResultBaseDTO, ICreatedResult
{
    [JsonIgnore]
    public CreatedAtRouteValues CreatedAtRouteValues => new()
    {
        Action = nameof(MatchesController.GetByIdAsync),
        Controller = nameof(MatchesController),
        RouteValues = new { id = Id }
    };

    /// <summary>
    /// osu! match id
    /// </summary>
    public long MatchId { get; set; }
}
