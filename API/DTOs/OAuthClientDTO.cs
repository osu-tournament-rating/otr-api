using API.Entities;

namespace API.DTOs;

public class OAuthClientDTO
{
    public int ClientId { get; set; }
    public string ClientSecret { get; set; } = null!;
    public string[] Scopes { get; set; } = [];
    public RateLimitOverrides RateLimitOverrides { get; set; } = new();
}
