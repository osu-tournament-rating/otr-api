namespace API.DTOs;

public class OAuthResponseDTO
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public long AccessExpiration { get; set; } = 3600; // 1 hour default
}
