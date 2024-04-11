using System.Security.Claims;
using API.Enums;

namespace API.Utilities;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Denotes the principal as having the admin role.
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, "admin");

    /// <summary>
    /// Denotes the principal as having the system role.
    /// </summary>
    public static bool IsSystem(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, "system");

    /// <summary>
    /// Denotes the principal as having the user role.
    /// </summary>
    public static bool IsUser(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, "user");

    /// <summary>
    /// Denotes the principal as having the client role.
    /// </summary>
    public static bool IsClient(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, "client");

    /// <summary>
    /// Denotes the principal as having the verifier role.
    /// </summary>
    public static bool IsMatchVerifier(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, "verifier");

    /// <summary>
    /// Returns the appropriate <see cref="MatchVerificationSource"/> enum for the principle
    /// </summary>
    public static MatchVerificationSource? VerificationSource(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.IsMatchVerifier()
            ? MatchVerificationSource.MatchVerifier
            : null;

    private static bool IsInRole(ClaimsPrincipal claimsPrincipal, string role)
    {
        return claimsPrincipal.IsInRole(role) || claimsPrincipal.HasClaim("role", role);
    }
}
