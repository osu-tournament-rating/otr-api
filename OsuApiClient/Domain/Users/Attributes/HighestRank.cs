using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Newtonsoft.Json;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents a user's highest recorded global rank
/// </summary>
[AutoMap(typeof(HighestRankJsonModel))]
[SuppressMessage("ReSharper", "CommentTypo")]
public class HighestRank : IModel
{
    [JsonProperty("rank")]
    public int Rank { get; set; }

    [JsonProperty("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
