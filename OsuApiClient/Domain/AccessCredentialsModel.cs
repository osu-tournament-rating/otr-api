using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels;

namespace OsuApiClient.Domain;

/// <summary>
/// Represents a set of osu! API access credentials
/// </summary>
/// <remarks>See <a href="https://osu.ppy.sh/docs/index.html#client-credentials-grant">Client Credentials Grant</a></remarks>
[AutoMap(typeof(AccessCredentialsJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class AccessCredentialsModel : IModel
{
    /// <summary>
    /// The type of token, this should always be "Bearer"
    /// </summary>
    public string TokenType { get; init; } = null!;

    /// <summary>
    /// The number of seconds the token will be valid for
    /// </summary>
    public long ExpiresIn { get; init; }

    /// <summary>
    /// The access token
    /// </summary>
    public string AccessToken { get; init; } = null!;

    /// <summary>
    /// The refresh token
    /// </summary>
    public string? RefreshToken { get; init; }
}
