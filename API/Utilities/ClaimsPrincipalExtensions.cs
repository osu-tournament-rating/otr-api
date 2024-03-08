using System.Security.Claims;

namespace API.Utilities;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Denotes the principal as having the admin role.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.IsInRole("admin");

    /// <summary>
    /// Denotes the principal as having the system role.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsSystem(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.IsInRole("system");
    /// <summary>
    /// Denotes the principal as having the user role.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsUser(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.IsInRole("user");
    /// <summary>
    /// Denotes the principal as having the client role.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsClient(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.IsInRole("client");
    /// <summary>
    /// Denotes the principal as having the verifier role.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsMatchVerifier(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.IsInRole("verifier");
}
