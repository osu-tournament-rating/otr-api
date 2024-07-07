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

    /// <summary>
    /// Regex filters that define valid <see cref="Database.Entities.Match"/> <see cref="Database.Entities.Match.Name"/>
    /// conventions
    /// </summary>
    public static readonly List<string> MatchNamePatterns = [@"^[^\n\r]*(\(.+\)\s*vs\.?\s*\(.+\)).*$"];

    /// <summary>
    /// Percentage threshold (0.0 to 1.0) that a <see cref="Database.Entities.Tournament"/>'s
    /// <see cref="Database.Entities.Tournament.Matches"/> having a
    /// <see cref="Database.Enums.Verification.VerificationStatus"/> of
    /// <see cref="Database.Enums.Verification.VerificationStatus.PreVerified"/> or
    /// <see cref="Database.Enums.Verification.VerificationStatus.Verified"/> must meet or exceed to be considered valid
    /// </summary>
    public const double TournamentVerifiedMatchesPercentageThreshold = 0.8;
}
