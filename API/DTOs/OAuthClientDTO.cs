namespace API.DTOs;

public class OAuthClientDTO
{
    public int ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string[] Scopes { get; set; }
}