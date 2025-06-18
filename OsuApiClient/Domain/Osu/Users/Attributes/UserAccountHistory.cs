using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents a disciplinary action taken against a user
/// </summary>
[AutoMap(typeof(UserAccountHistoryJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class UserAccountHistory : IModel
{
    /// <summary>
    /// Description of the action
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Id of the action
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Length of time for the punishment
    /// </summary>
    public int Length { get; init; }

    /// <summary>
    /// Denotes if the punishment is permanent
    /// </summary>
    public bool Permanent { get; init; }

    /// <summary>
    /// Timestamp for when the action was taken
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// The type of action taken
    /// </summary>
    /// <example>
    /// Could be one of:
    /// "silence", "restriction"
    /// </example>
    public string? Type { get; init; }
}
