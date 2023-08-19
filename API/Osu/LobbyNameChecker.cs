namespace API.Osu;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class LobbyNameChecker
{
	// List of regex patterns
	private static readonly List<string> patterns = new List<string>
	{
		@".+?:\s?\((.+?)\)\s?vs\.?\s?\((.+?)\)",
		@"""[\w\s!:\-]+?:\s?([^()]+?)\s?vs\s?([^()]+?)""",
		@"\([\w\s!:\-]+?\):\s?\(([^)]+)\)\s?vs\s?\(([^)]+)\)",
		@"\([\w\s!:\-]+?:\s?([^)]+?)\)\s?vs\s?\(([^)]+)\)",
		@".+?:\s?\(([^)]+)\)\s?vs\.\s?\(([^)]+)\)",
		@".+?:\s?[^()]+?\s?vs\.?\s?[^()]+?$"
	};

	// Method to test if a name matches any of the patterns
	public static bool IsNameValid(string name) => patterns.Any(pattern => Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase));

	// Method to test a specific pattern against a name (useful for debugging or specific tests)
	public static bool TestSpecificPattern(string name, int patternIndex)
	{
		if (patternIndex < 0 || patternIndex >= patterns.Count)
		{
			throw new ArgumentOutOfRangeException(nameof(patternIndex), "Invalid pattern index.");
		}

		return Regex.IsMatch(name, patterns[patternIndex], RegexOptions.IgnoreCase);
	}
}