using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents a user's highest recorded global rank
/// </summary>
[AutoMap(typeof(HighestRankJsonModel))]
[SuppressMessage("ReSharper", "CommentTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class HighestRank : IModel
{
    /// <summary>
    /// Highest recorded global rank
    /// </summary>
    public int Rank { get; init; }

    /// <summary>
    /// Timestamp for when the <see cref="Rank"/> was recorded
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}