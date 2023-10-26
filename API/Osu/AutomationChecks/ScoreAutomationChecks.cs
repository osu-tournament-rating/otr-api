using API.Entities;

namespace API.Osu.AutomationChecks;

public static class ScoreAutomationChecks
{
	public static bool PassesAutomationChecks(MatchScore score)
	{
		return PassesModsCheck(score);
	}

	public static bool PassesModsCheck(MatchScore score)
	{
		if (score.EnabledMods == null)
		{
			return true;
		}
		
		return !AutomationConstants.UnallowedMods.Any(unallowedMod => score.EnabledModsEnum!.Value.HasFlag(unallowedMod));
	}
}