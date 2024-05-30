using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents a disciplinary action taken against a user
/// </summary>
[AutoMap(typeof(UserAccountHistoryJsonModel))]
public class UserAccountHistory : IModel
{
    /// <summary>
    /// Description of the action
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Id of the action
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Length of time for the punishment
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Denotes if the punishment is permanent
    /// </summary>
    public bool Permanent { get; set; }

    /// <summary>
    /// Timestamp for when the action was taken
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The type of action taken
    /// </summary>
    /// <example>
    /// Could be one of:
    /// "silence", "restriction"
    /// </example>
    public string? Type { get; set; }
}
