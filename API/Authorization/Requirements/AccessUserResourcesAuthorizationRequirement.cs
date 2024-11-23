using Microsoft.AspNetCore.Authorization;

namespace API.Authorization.Requirements;

/// <summary>
/// Requirement that controls authorization for the <see cref="AuthorizationPolicies.AccessUserResources"/> policy
/// </summary>
public class AccessUserResourcesAuthorizationRequirement : IAuthorizationRequirement;
