using System.Text.Json;
using System.Text.Json.Serialization;
using API.Entities;

namespace API.Utilities;

public static class RateLimitOverridesSerializer
{
    private static readonly JsonSerializerOptions s_serializerOptions =
        new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public static string Serialize(RateLimitOverrides data) =>
        JsonSerializer.Serialize(data, s_serializerOptions);
}
