using System.Text;
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

	public static bool IsNameValid(string name, string abbreviation)
	{
		var abbreviationLobbyTitlePrefix = new StringBuilder(abbreviation)
		         .Append(':')
		         .ToString();
		// We don't want a name where there is a space immediately before the colon
		return name.StartsWith(abbreviationLobbyTitlePrefix) && patterns.Any(pattern => Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase));
	}
}