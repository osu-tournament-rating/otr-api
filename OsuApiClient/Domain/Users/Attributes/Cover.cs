using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents a user's cover image
/// </summary>
[AutoMap(typeof(CoverJsonModel))]
public class Cover : IModel
{
    /// <summary>
    /// No description
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Cover image url
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// No description
    /// </summary>
    public string? CustomUrl { get; set; }
}
