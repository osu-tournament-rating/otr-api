using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents a user's country
/// </summary>
[AutoMap(typeof(CountryJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class Country : IModel
{
    /// <summary>
    /// Two-letter ISO country code
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Country name
    /// </summary>
    public string? Name { get; init; }
}
