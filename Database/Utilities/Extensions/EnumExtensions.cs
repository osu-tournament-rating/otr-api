using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace Database.Utilities.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Denotes if the enum is able to be used to fetch the osu! API
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public static bool IsFetchable(this Ruleset ruleset) =>
        ruleset switch
        {
            Ruleset.Mania4k => false,
            Ruleset.Mania7k => false,
            _ => true
        };

    /// <summary>
    /// Returns the opposite team
    /// </summary>
    public static Team OppositeTeam(this Team team) =>
        team switch
        {
            Team.Blue => Team.Red,
            Team.Red => Team.Blue,
            _ => Team.NoTeam
        };
}
