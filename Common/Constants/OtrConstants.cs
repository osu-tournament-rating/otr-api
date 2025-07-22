using Common.Enums;

namespace Common.Constants;

public static class OtrConstants
{
    /// <summary>
    /// Mods that are ineligible for ratings
    /// </summary>
    public static readonly IEnumerable<Mods> InvalidMods =
        [Mods.SuddenDeath, Mods.Perfect, Mods.Relax, Mods.Autoplay, Mods.Relax2];

    /// <summary>
    /// Lowest (non-inclusive) <see cref="Database.Entities.GameScore.Score"/> for a
    /// <see cref="Database.Entities.GameScore"/> to be considered valid
    /// </summary>
    public const int ScoreMinimum = 1_000;

    /// <summary>
    /// Regex filters that define valid <see cref="Database.Entities.Match"/> <see cref="Database.Entities.Match.Name"/>
    /// conventions
    /// </summary>
    public static readonly List<string> MatchNamePatterns = [@"^[^]*(\(.+\)\s*vs\.?\s*\(.+\)).*$"];
}
