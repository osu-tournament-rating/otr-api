using System.Text.RegularExpressions;

namespace API.Osu;

public static class LobbyNameChecker
{
	// List of regex patterns for lobby names
	private static readonly List<string> patterns = new()
	{
		@"^[\w\.""'-]+:(\s*.+\|)?\s*\([^\)]+\)\s+vs\.?\s+\([^\)]+\)$",
		@"^[\w\.""'-]+:(\s*.+\|)?\s*\([^\)]+\)\s+vs\.?\s+[^()]+$",
		@"^[\w\.""'-]+:(\s*.+\|)?\s*[^()]+\s+vs\.?\s+\([^\)]+\)$",
		@"^[\w\.""'-]+:(\s*.+\|)?\s*[^()]+\s+vs\.?\s+[^()]+$",
		@"^""[a-zA-Z0-9\s]+: \([a-zA-Z0-9""]+\) vs \([a-zA-Z0-9""]+\)""$"
	};
	public static bool IsNameValid(string name) => patterns.Any(pattern => Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase));
}