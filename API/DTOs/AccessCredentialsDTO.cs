namespace API.DTOs;

/// <summary>
/// Represents access credentials and their expiry
/// </summary>
public class AccessCredentialsDTO
{
    /// <summary>
    /// Access token
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Lifetime of the access token in seconds
    /// </summary>
    public int? AccessExpiration { get; set; }

    /// <summary>
    /// Lifetime of the refresh token in seconds
    /// </summary>
    public int? RefreshExpiration { get; set; }
}
