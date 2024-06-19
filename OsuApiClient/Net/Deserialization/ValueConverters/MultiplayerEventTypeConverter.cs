using AutoMapper;
using OsuApiClient.Enums;

namespace OsuApiClient.Net.Deserialization.ValueConverters;

/// <summary>
/// Converts a string into its respective <see cref="MultiplayerEventType"/>
/// </summary>
public class MultiplayerEventTypeConverter : IValueConverter<string, MultiplayerEventType>
{
    public MultiplayerEventType Convert(string sourceMember, ResolutionContext context) =>
        Convert(sourceMember);

    public static MultiplayerEventType Convert(string value)
    {
        if (Enum.TryParse(value, true, out MultiplayerEventType result))
        {
            return result;
        }

        return value switch
        {
            "match-created" => MultiplayerEventType.MatchCreated,
            "match-disbanded" => MultiplayerEventType.MatchDisbanded,
            "player-joined" => MultiplayerEventType.PlayerJoined,
            "player-left" => MultiplayerEventType.PlayerLeft,
            "other" => MultiplayerEventType.Game,
            // This should never happen, but a fallback is ok for our use case
            _ => MultiplayerEventType.Unknown
        };
    }
}
