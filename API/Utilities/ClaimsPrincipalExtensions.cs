using System.Security.Claims;
using API.Enums;

namespace API.Utilities;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Denotes the principal as having the admin role.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, "admin");

    /// <summary>
    /// Denotes the principal as having the system role.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsSystem(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, "system");
    /// <summary>
    /// Denotes the principal as having the user role.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsUser(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, "user");

    /// <summary>
    /// Denotes the principal as having the client role.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsClient(this ClaimsPrincipal claimsPrincipal) => IsInRole(claimsPrincipal, "client");

    /// <summary>
    /// Denotes the principal as having the verifier role.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsMatchVerifier(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, "verifier");

    /// <summary>
    /// Denotes the principle as having either the verifier or admin role
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool CanVerifyMatches(this ClaimsPrincipal claimsPrincipal) =>
        (claimsPrincipal.IsAdmin() || claimsPrincipal.IsMatchVerifier());

    /// <summary>
    /// Returns the appropriate <see cref="MatchVerificationSource"/> enum for the principle
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static MatchVerificationSource? VerificationSource(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.IsAdmin())
        {
            return MatchVerificationSource.Admin;
        }

        if (claimsPrincipal.IsMatchVerifier())
        {
            return MatchVerificationSource.MatchVerifier;
        }

        return null;
    }

    private static bool IsInRole(ClaimsPrincipal claimsPrincipal, string role)
    {
        return claimsPrincipal.IsInRole(role) || claimsPrincipal.HasClaim("role", role);
    }
}
