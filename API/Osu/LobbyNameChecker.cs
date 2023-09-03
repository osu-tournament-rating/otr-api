using System.Text.RegularExpressions;

namespace API.Osu;

public static class LobbyNameChecker
{
	// List of regex patterns for lobby names
	private static readonly List<string> patterns = new()
	{
		@"^[\w\.""'-]+:(\s*.+\|)?\s*.*\s+vs\.?\s+.*$",
		@"^[\w\s\.""'-]+:(\s*.+\|)?\s*.*\s+vs\.?\s+.*$"
	};

	public static bool IsNameValid(string name)
	{
		// We don't want a name where there is a space immediately before the colon
		return !name.Contains(" :") && patterns.Any(pattern => Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase));
	}
}