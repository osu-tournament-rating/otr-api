using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Beatmaps;

namespace OsuApiClient.Domain.Osu.Beatmaps;

/// <summary>
/// Represents an owner of a <see cref="Beatmap"/>
/// </summary>
[AutoMap(typeof(BeatmapOwnerJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class BeatmapOwner : IModel
{
    /// <summary>
    /// osu! Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// osu! username
    /// </summary>
    public string Username { get; set; } = string.Empty;
}
