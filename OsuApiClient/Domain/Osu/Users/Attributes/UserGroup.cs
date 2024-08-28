using System.Diagnostics.CodeAnalysis;
using Database.Enums;
using OsuApiClient.Domain.Osu.Site;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Describes a Group membership of a User.
/// It contains all of the attributes of the <see cref="Group"/>, in addition to what is listed here
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class UserGroup : Group
{
    /// <summary>
    /// Ruleset(s) associated with this membership
    /// </summary>
    /// <remarks>Null if <see cref="Group.HasRulesets"/> is set to false</remarks>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public Ruleset[]? Rulesets { get; init; }
}
