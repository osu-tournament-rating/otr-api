namespace API.Osu.AutomationChecks;

public static class AutomationConstants
{
	public static readonly OsuEnums.Mods[] UnallowedMods =
	{
		OsuEnums.Mods.TouchDevice,
		OsuEnums.Mods.SuddenDeath,
		OsuEnums.Mods.Relax,
		OsuEnums.Mods.Autoplay,
		OsuEnums.Mods.SpunOut,
		OsuEnums.Mods.Relax2, // Autopilot
		OsuEnums.Mods.Perfect,
		OsuEnums.Mods.Random,
		OsuEnums.Mods.Cinema,
		OsuEnums.Mods.Target
	};
}