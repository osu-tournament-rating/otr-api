using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents a user's cover image
/// </summary>
[AutoMap(typeof(CoverJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class Cover : IModel
{
    /// <summary>
    /// No description
    /// </summary>
    public int? Id { get; init; }

    /// <summary>
    /// Cover image url
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// No description
    /// </summary>
    public string? CustomUrl { get; init; }
}
