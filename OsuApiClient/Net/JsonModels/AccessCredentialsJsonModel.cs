using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels;

/// <summary>
/// JSON Model for osu! API access credentials
/// </summary>
public class AccessCredentialsJsonModel : JsonModelBase
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
