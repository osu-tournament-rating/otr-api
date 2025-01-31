using AutoMapper;
using Database.Enums;

namespace OsuApiClient.Net.Deserialization.ValueConverters;

/// <summary>
/// Converts a string into its respective <see cref="TeamType"/>
/// </summary>
public class TeamTypeConverter : IValueConverter<string, TeamType>
{
    public TeamType Convert(string sourceMember, ResolutionContext context) =>
        Convert(sourceMember);

    public static TeamType Convert(string value)
    {
        if (Enum.TryParse(value, true, out TeamType result))
        {
            return result;
        }

        return value switch
        {
            "head-to-head" => TeamType.HeadToHead,
            "tag-coop" => TeamType.TagCoop,
            "team-vs" => TeamType.TeamVs,
            "tag-team-vs" => TeamType.TagTeamVs,
            _ => throw new ArgumentException($"Failed to convert value '{value}' to {nameof(TeamType)}")
        };
    }
}
