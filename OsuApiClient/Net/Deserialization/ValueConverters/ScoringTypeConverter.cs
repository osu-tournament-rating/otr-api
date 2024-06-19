using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Database.Enums;

namespace OsuApiClient.Net.Deserialization.ValueConverters;

/// <summary>
/// Converts a string into its respective <see cref="MultiplayerEventType"/>
/// </summary>
public class ScoringTypeConverter : IValueConverter<string, ScoringType>
{
    public ScoringType Convert(string sourceMember, ResolutionContext context) => Convert(sourceMember);

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static ScoringType Convert(string value)
    {
        if (Enum.TryParse(value, true, out ScoringType result))
        {
            return result;
        }

        return value switch
        {
            "score" => ScoringType.Score,
            "accuracy" => ScoringType.Accuracy,
            "combo" => ScoringType.Combo,
            "scorev2" => ScoringType.ScoreV2,
            // This should never happen, but a fallback is ok for our use case
            _ => ScoringType.Score
        };
    }
}
