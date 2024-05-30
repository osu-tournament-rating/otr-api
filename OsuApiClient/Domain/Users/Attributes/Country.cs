using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents a user's country
/// </summary>
[AutoMap(typeof(CountryJsonModel))]
public class Country : IModel
{
    /// <summary>
    /// Two-letter ISO country code
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Country name
    /// </summary>
    public string? Name { get; set; }
}
