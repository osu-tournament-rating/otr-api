using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents access credentials and their expiry
/// </summary>
public class AccessCredentialsDTO
{
    /// <summary>
    /// Access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Lifetime of the access token in seconds
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Token type
    /// </summary>
    [UsedImplicitly]
    public string TokenType { get; set; } = "Bearer";
}
