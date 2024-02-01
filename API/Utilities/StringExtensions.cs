namespace API.Utilities;

public static class StringExtensions
{
	public static string ReplaceUnderscores(this string s) => s.Replace('_', ' ');
}