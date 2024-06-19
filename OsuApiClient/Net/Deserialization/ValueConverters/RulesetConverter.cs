using AutoMapper;
using Database.Enums;

namespace OsuApiClient.Net.Deserialization.ValueConverters;

/// <summary>
/// Converts a string into its respective <see cref="Ruleset"/>
/// </summary>
public class RulesetConverter : IValueConverter<string, Ruleset>
{
    public Ruleset Convert(string sourceMember, ResolutionContext context) =>
        Convert(sourceMember);

    public static Ruleset Convert(string value)
    {
        if (Enum.TryParse(value, true, out Ruleset result))
        {
            return result;
        }

        return value switch
        {
            "osu" => Ruleset.Standard,
            "taiko" => Ruleset.Taiko,
            "fruits" => Ruleset.Catch,
            "mania" => Ruleset.Mania,
            // This should never happen, but using standard as a fallback is ok for our use case
            _ => Ruleset.Standard
        };
    }
}
