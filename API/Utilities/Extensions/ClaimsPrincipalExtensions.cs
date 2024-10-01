using System.Security.Claims;
using API.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace API.Utilities.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Denotes the principal as having the admin role.
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Admin);

    /// <summary>
    /// Denotes the principal as having the system role.
    /// </summary>
    public static bool IsSystem(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, OtrClaims.Roles.System);

    /// <summary>
    /// Denotes the principal as having the user role.
    /// </summary>
    public static bool IsUser(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, OtrClaims.Roles.User);

    /// <summary>
    /// Denotes the principal as having the client role.
    /// </summary>
    public static bool IsClient(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, OtrClaims.Roles.Client);

    /// <summary>
    /// Denotes the principal as having the verifier role.
    /// </summary>
    public static bool IsMatchVerifier(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Verifier);

    /// <summary>
    /// Denotes the principle as having the whitelist role
    /// </summary>
    public static bool IsWhitelisted(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Whitelist);

    /// <summary>
    /// Gets the issuer id of the principle
    /// </summary>
    /// <returns>The issuer id of the principle, or null if not properly logged in</returns>
    public static int? AuthorizedIdentity(this ClaimsPrincipal claimsPrincipal)
    {
        var id = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iss)?.Value;
        if (id == null)
        {
            return null;
        }

        if (!int.TryParse(id, out var idInt))
        {
            return null;
        }

        return idInt;
    }

    private static bool IsInRole(ClaimsPrincipal claimsPrincipal, string role)
    {
        return claimsPrincipal.IsInRole(role) || claimsPrincipal.HasClaim("role", role);
    }
}
