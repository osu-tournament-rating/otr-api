using AutoMapper;
using OsuApiClient.Net.JsonModels.Site;

namespace OsuApiClient.Domain.Site;

/// <summary>
/// Represents a group on the osu! website
/// </summary>
/// <example>Global Moderation Team (GMT), Beatmap Nominator (BN)</example>
[AutoMap(typeof(GroupJsonModel))]
public class Group : IModel
{
    /// <summary>
    /// Color of the group
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Whether this group displays a listing at `/groups/{id}`
    /// </summary>
    public bool HasListing { get; set; }

    /// <summary>
    /// Denotes whether this group associates any ruleset(s) with users' memberships
    /// </summary>
    public bool HasPlayModes { get; set; }

    /// <summary>
    /// Id of the group
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique string to identify the group
    /// </summary>
    public string Identifier { get; set; } = null!;

    /// <summary>
    /// Denotes whether members of this group are considered probationary
    /// </summary>
    public bool IsProbationary { get; set; }

    /// <summary>
    /// Name of the group
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Short name of the group for display
    /// </summary>
    public string ShortName { get; set; } = null!;
}
