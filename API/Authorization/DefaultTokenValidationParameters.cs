using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Authorization;

/// <summary>
/// Configures the default <see cref="TokenValidationParameters"/> for authorization
/// </summary>
public static class DefaultTokenValidationParameters
{
    /// <summary>
    /// Gets the default <see cref="TokenValidationParameters"/> for authorization
    /// </summary>
    public static TokenValidationParameters Get(string issuer, string key, string audience) => new()
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        NameClaimType = OtrClaims.Subject,
        RoleClaimType = OtrClaims.Role,
        ClockSkew = TimeSpan.Zero
    };
}
