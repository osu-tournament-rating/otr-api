using System.Diagnostics.CodeAnalysis;

namespace OsuApiClient.Domain.Site;

/// <summary>
/// Represents a group on the osu! website
/// </summary>
/// <example>Global Moderation Team (GMT), Beatmap Nominator (BN)</example>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class Group : IModel
{
    /// <summary>
    /// Color of the group
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    /// Whether this group displays a listing at `/groups/{id}`
    /// </summary>
    public bool HasListing { get; init; }

    /// <summary>
    /// Denotes whether this group associates any ruleset(s) with users' memberships
    /// </summary>
    public bool HasPlayModes { get; init; }

    /// <summary>
    /// Id of the group
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Unique string to identify the group
    /// </summary>
    public string Identifier { get; init; } = null!;

    /// <summary>
    /// Denotes whether members of this group are considered probationary
    /// </summary>
    public bool IsProbationary { get; init; }

    /// <summary>
    /// Name of the group
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Short name of the group for display
    /// </summary>
    public string ShortName { get; init; } = null!;
}
