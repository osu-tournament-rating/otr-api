using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents a user achievement
/// </summary>
[AutoMap(typeof(UserAchievementJsonModel))]
public class UserAchievement : IModel
{
    /// <summary>
    /// Timestamp for when the achievement was obtained
    /// </summary>
    public DateTimeOffset AchievedAt { get; set; }

    /// <summary>
    /// Id of the achievement
    /// </summary>
    public int AchievementId { get; set; }
}
