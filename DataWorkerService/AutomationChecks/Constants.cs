using Database.Enums;

namespace DataWorkerService.AutomationChecks;

/// <summary>
/// Constant values used by an <see cref="IAutomationCheck{TEntity}"/>
/// </summary>
public static class Constants
{
    /// <summary>
    /// Mods that are ineligible for ratings
    /// </summary>
    public static readonly IEnumerable<Mods> InvalidMods =
        new[] { Mods.SuddenDeath, Mods.Perfect, Mods.Relax, Mods.Autoplay, Mods.SpunOut };

    /// <summary>
    /// Lowest (non-inclusive) <see cref="Database.Entities.GameScore.Score"/> for a
    /// <see cref="Database.Entities.GameScore"/> to be considered valid
    /// </summary>
    public const long ScoreMinimum = 1_000;
}
