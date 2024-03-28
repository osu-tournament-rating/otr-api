using API.Entities;
using AutoMapper;
using AutoMapper.Configuration.Annotations;

namespace API.DTOs;

[AutoMap(typeof(OAuthClient))]
public class OAuthClientDTO
{
    [SourceMember(nameof(OAuthClient.Id))]
    public int ClientId { get; set; }

    [SourceMember(nameof(OAuthClient.Secret))]
    public string ClientSecret { get; set; } = null!;

    public string[] Scopes { get; set; } = [];

    public RateLimitOverrides RateLimitOverrides { get; set; } = new();
}
