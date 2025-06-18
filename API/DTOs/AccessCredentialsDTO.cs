using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents access credentials and their expiry
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class AccessCredentialsDTO
{
    /// <summary>
    /// Access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Lifetime of the access token in seconds
    /// </summary>
    public int ExpiresIn { get; set; }
}
