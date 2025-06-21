using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents a user achievement
/// </summary>
[AutoMap(typeof(UserAchievementJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
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
