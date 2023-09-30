using System.Text;
using System.Text.RegularExpressions;

namespace API.Osu;

public static class LobbyNameChecker
{
	// List of regex patterns for lobby names
	private static readonly List<string> patterns = new()
	{
		@"^[^\n\r]*(\(.+\)\s*vs\.?\s*\(.+\)).*$",
	};

	public static bool IsNameValid(string name, string abbreviation)
	{
		return name.StartsWith(abbreviation) && patterns.Any(pattern => Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase));
	}
}