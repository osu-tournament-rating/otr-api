using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents a user achievement
/// </summary>
[AutoMap(typeof(UserAchievementJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class UserAchievement : IModel
{
    /// <summary>
    /// Timestamp for when the achievement was obtained
    /// </summary>
    public DateTimeOffset AchievedAt { get; init; }

    /// <summary>
    /// Id of the achievement
    /// </summary>
    public int AchievementId { get; init; }
}
