using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a created OAuth client
/// </summary>
/// <remarks>The only time the client secret is available is when a new client is created</remarks>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class OAuthClientCreatedDTO : OAuthClientDTO
{
    /// <summary>
    /// Client secret of the client
    /// </summary>
    public string ClientSecret { get; set; } = null!;
}
