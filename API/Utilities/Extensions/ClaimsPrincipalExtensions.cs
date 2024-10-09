using System.Security.Claims;
using API.Authorization;
using Database.Entities;

namespace API.Utilities.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Denotes the principal as having the <see cref="OtrClaims.Roles.Admin"/> <see cref="OtrClaims.Role"/>
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Admin);

    /// <summary>
    /// Denotes the principal as having the <see cref="OtrClaims.Roles.User"/> <see cref="OtrClaims.Role"/>
    /// </summary>
    public static bool IsUser(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.User);

    /// <summary>
    /// Denotes the principal as having the <see cref="OtrClaims.Roles.Client"/> <see cref="OtrClaims.Role"/>
    /// </summary>
    public static bool IsClient(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Client);

    /// <summary>
    /// Denotes the principal as having the <see cref="OtrClaims.Roles.Verifier"/> <see cref="OtrClaims.Role"/>
    /// </summary>
    public static bool IsMatchVerifier(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Verifier);

    /// <summary>
    /// Denotes the principle as having the <see cref="OtrClaims.Roles.Whitelist"/> <see cref="OtrClaims.Role"/>
    /// </summary>
    public static bool IsWhitelisted(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Whitelist);

    /// <summary>
    /// Gets the <see cref="OtrClaims.TokenType"/> of the principle
    /// </summary>
    public static string? GetTokenType(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.FindFirst(OtrClaims.TokenType)?.Value;

    /// <summary>
    /// Gets the <see cref="RateLimitOverrides"/> of the principle if available
    /// </summary>
    public static RateLimitOverrides? GetRateLimitOverrides(this ClaimsPrincipal claimsPrincipal)
    {
        var raw = claimsPrincipal.FindFirst(OtrClaims.RateLimitOverrides)?.Value;
        return string.IsNullOrEmpty(raw) ? null : RateLimitOverridesSerializer.Deserialize(raw);
    }

    /// <summary>
    /// Gets the <see cref="OtrClaims.Subject"/> id of the principle
    /// </summary>
    public static int GetSubjectId(this ClaimsPrincipal claimsPrincipal)
    {
        var sub = claimsPrincipal.Identity?.Name ?? claimsPrincipal.FindFirst(OtrClaims.Subject)?.Value;
        if (!int.TryParse(sub, out var idInt))
        {
            throw new ArgumentException(
                $"The claims principle did not contain a valid {OtrClaims.Subject} claim"
            );
        }

        return idInt;
    }

    private static bool IsInRole(ClaimsPrincipal claimsPrincipal, string role)
    {
        return claimsPrincipal.IsInRole(role) || claimsPrincipal.HasClaim(OtrClaims.Role, role);
    }
}
