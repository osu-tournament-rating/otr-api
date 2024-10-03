using System.Text.Json.Serialization;

namespace API.DTOs;

/// <summary>
/// Represents access credentials and their expiry
/// </summary>
public class AccessCredentialsDTO
{
    /// <summary>
    /// Access token
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccessToken { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Lifetime of the access token in seconds
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? AccessExpiration { get; set; }

    /// <summary>
    /// Lifetime of the refresh token in seconds
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? RefreshExpiration { get; set; }
}
