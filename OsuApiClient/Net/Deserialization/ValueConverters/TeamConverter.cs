using AutoMapper;
using Common.Enums;

namespace OsuApiClient.Net.Deserialization.ValueConverters;

/// <summary>
/// Converts a string into its respective <see cref="Team"/>
/// </summary>
public class TeamConverter : IValueConverter<string, Team>
{
    public Team Convert(string sourceMember, ResolutionContext context) =>
        Convert(sourceMember);

    public static Team Convert(string value)
    {
        if (Enum.TryParse(value, true, out Team result))
        {
            return result;
        }

        return value switch
        {
            "none" => Team.NoTeam,
            "blue" => Team.Blue,
            "red" => Team.Red,
            // This should never happen, but a fallback is ok for our use case
            _ => Team.NoTeam
        };
    }
}
