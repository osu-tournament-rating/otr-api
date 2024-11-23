using Microsoft.AspNetCore.Authorization;

namespace API.Authorization.Requirements;

/// <summary>
/// Requirement that controls authorization for the <see cref="AuthorizationPolicies.Whitelist"/> policy
/// </summary>
public class WhitelistAuthorizationRequirement : IAuthorizationRequirement;
