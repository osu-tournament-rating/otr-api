using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents a user's cover image
/// </summary>
[AutoMap(typeof(CoverJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
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
