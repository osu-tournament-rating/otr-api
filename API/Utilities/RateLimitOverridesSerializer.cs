using System.Text.Json;
using System.Text.Json.Serialization;
using API.Entities;

namespace API.Utilities;

/// <summary>
/// Custom serializer for encoding / decoding <see cref="RateLimitOverrides"/>
/// into and out of a JSON Web Token (JWT)
/// </summary>
public static class RateLimitOverridesSerializer
{
    private static readonly JsonSerializerOptions s_serializerOptions =
        new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

    /// <summary>
    /// Serialize an instance of <see cref="RateLimitOverrides"/> for use with encoding
    /// into a JSON Web Token (JWT)
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Serialize(RateLimitOverrides data)
    {
        var serializedData = JsonSerializer.Serialize(data, s_serializerOptions);
        return serializedData.Equals("{}") ? string.Empty : serializedData;
    }

    /// <summary>
    /// Deserialize a string representation of <see cref="RateLimitOverrides"/>
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static RateLimitOverrides? Deserialize(string data) =>
        JsonSerializer.Deserialize<RateLimitOverrides>(data, s_serializerOptions);
}
