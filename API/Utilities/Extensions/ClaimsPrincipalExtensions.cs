using System.Security.Claims;
using Database.Enums;
using Microsoft.IdentityModel.JsonWebTokens;

namespace API.Utilities.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Denotes the principal as having the admin role.
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Admin);

    /// <summary>
    /// Denotes the principal as having the system role.
    /// </summary>
    public static bool IsSystem(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, OtrClaims.System);

    /// <summary>
    /// Denotes the principal as having the user role.
    /// </summary>
    public static bool IsUser(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, OtrClaims.User);

    /// <summary>
    /// Denotes the principal as having the client role.
    /// </summary>
    public static bool IsClient(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, OtrClaims.Client);

    /// <summary>
    /// Denotes the principal as having the verifier role.
    /// </summary>
    public static bool IsMatchVerifier(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Verifier);

    /// <summary>
    /// Denotes the principle as having the whitelist role
    /// </summary>
    public static bool IsWhitelisted(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Whitelist);

    /// <summary>
    /// Returns the appropriate <see cref="Old_MatchVerificationSource"/> enum for the principle
    /// </summary>
    public static Old_MatchVerificationSource? VerificationSource(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.IsMatchVerifier()
            ? Old_MatchVerificationSource.MatchVerifier
            : null;

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
