using System.Diagnostics.CodeAnalysis;
using OsuApiClient.Domain.Site;

namespace OsuApiClient.Domain.Users.Attributes;

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
    /// <remarks>Null if <see cref="Group.HasPlayModes"/> is set to false</remarks>
    public string[]? PlayModes { get; init; }
}
