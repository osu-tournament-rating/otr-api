using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Common.Enums;

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
            _ => throw new ArgumentException($"Failed to convert value '{value}' to {nameof(ScoringType)}")
        };
    }
}
