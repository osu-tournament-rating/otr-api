using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels;

/// <summary>
/// JSON Model for osu! API access credentials
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#authentication
/// Last accessed May 2024
/// </copyright>
public class AccessCredentialsJsonModel
{
    [JsonProperty("token_type")]
    public string TokenType { get; set; } = null!;

    [JsonProperty("expires_in")]
    public long ExpiresIn { get; set; }

    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = null!;

    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; set; }
}
