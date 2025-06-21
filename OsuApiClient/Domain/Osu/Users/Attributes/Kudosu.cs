using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents information about a user's kudosu
/// </summary>
[AutoMap(typeof(KudosuJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class Kudosu : IModel
{
    /// <summary>
    /// Number of available kudosu
    /// </summary>
    public int Available { get; init; }

    /// <summary>
    /// Total number of kudosu
    /// </summary>
    public int Total { get; init; }
}
