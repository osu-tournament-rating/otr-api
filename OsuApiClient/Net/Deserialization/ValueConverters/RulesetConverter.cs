using AutoMapper;
using Common.Enums.Enums;

namespace OsuApiClient.Net.Deserialization.ValueConverters;

/// <summary>
/// Converts a string into its respective <see cref="Ruleset"/>
/// </summary>
public class RulesetConverter : IValueConverter<string, Ruleset>
{
    public Ruleset Convert(string sourceMember, ResolutionContext context) =>
        Convert(sourceMember);

    public static Ruleset Convert(string value, string? variant = null)
    {
        if (Enum.TryParse(value, true, out Ruleset result))
        {
            return result;
        }

        return (value, variant) switch
        {
            ("osu", null) => Ruleset.Osu,
            ("taiko", null) => Ruleset.Taiko,
            ("fruits", null) => Ruleset.Catch,
            ("mania", null) => Ruleset.ManiaOther,
            ("mania", "4k") => Ruleset.Mania4k,
            ("mania", "7k") => Ruleset.Mania7k,
            // This should never happen, but using standard as a fallback is ok for our use case
            _ => throw new ArgumentException($"Failed to convert value '{value}' to {nameof(Ruleset)}")
        };
    }
}
