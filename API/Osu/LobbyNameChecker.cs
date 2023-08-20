namespace API.Osu;

using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class LobbyNameChecker
{
	// List of regex patterns for lobby names
	private static readonly List<string> patterns = new()
	{
		@".+?:\s?\((.+?)\)\s?vs\.?\s?\((.+?)\)",
		@"""[\w\s!:\-]+?:\s?([^()]+?)\s?vs\s?([^()]+?)""",
		@"\([\w\s!:\-]+?\):\s?\(([^)]+)\)\s?vs\s?\(([^)]+)\)",
		@"\([\w\s!:\-]+?:\s?([^)]+?)\)\s?vs\s?\(([^)]+)\)",
		@".+?:\s?\(([^)]+)\)\s?vs\.\s?\(([^)]+)\)",
		@".+?:\s?[^()]+?\s?vs\.?\s?[^()]+?$",
		@".+?:.*?\((.+?)\)\s?vs\.?\s?\((.+?)\)"
	};

	public static bool IsNameValid(string name) => patterns.Any(pattern => Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase));
}