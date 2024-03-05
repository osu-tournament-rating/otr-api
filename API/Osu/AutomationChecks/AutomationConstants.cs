namespace API.Osu.AutomationChecks;

public static class AutomationConstants
{
    public static readonly OsuEnums.Mods[] UnallowedMods =
    {
        OsuEnums.Mods.SuddenDeath,
        OsuEnums.Mods.Perfect,
        OsuEnums.Mods.Relax,
        OsuEnums.Mods.Relax2, // Autopilot
        OsuEnums.Mods.SpunOut,
    };
}
