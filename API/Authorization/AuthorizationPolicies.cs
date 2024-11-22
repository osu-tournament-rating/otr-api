namespace API.Authorization;

/// <summary>
/// String constants that represent authorization policies
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// Policy that allows access from the user that owns the resource as well
    /// as any <see cref="OtrClaims.Roles.Admin"/> users
    /// </summary>
    public const string AccessUserResources = "AccessUserResources";

    /// <summary>
    /// Collection of all <see cref="AuthorizationPolicies"/>
    /// </summary>
    public static readonly string[] Policies =
    [
        AccessUserResources
    ];

    /// <summary>
    /// Gets the description of an authorization policy
    /// </summary>
    public static string GetDescription(string policy) =>
        policy switch
        {
            AccessUserResources =>
                "Policy that allows access from the user that owns the resource as well as any admin users",
            _ => "No description available."
        };
}
