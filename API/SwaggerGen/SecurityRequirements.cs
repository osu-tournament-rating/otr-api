using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace API.SwaggerGen;

/// <summary>
/// Contains <see cref="OpenApiSecurityRequirement"/>s used within the API
/// </summary>
public static class SecurityRequirements
{
    /// <summary>
    /// Id of the <see cref="OpenApiSecurityRequirement"/> for OAuth
    /// </summary>
    public const string OAuthSecurityRequirementId = "OAuth2";

    /// <summary>
    /// Creates a default <see cref="OpenApiSecurityRequirement"/> with a reference to the "BearerAuth"
    /// security definition
    /// </summary>
    public static OpenApiSecurityRequirement BearerSecurityRequirement => new()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
            },
            new List<string>()
        }
    };

    /// <summary>
    /// Creates a <see cref="OpenApiSecurityRequirement"/> with a reference to the "OAuth2" security definition
    /// </summary>
    /// <param name="roles">
    /// Optional list of required <see cref="API.Authorization.OtrClaims.Roles"/>
    /// </param>
    public static OpenApiSecurityRequirement OAuthSecurityRequirement(IEnumerable<string>? roles = null) => new()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = OAuthSecurityRequirementId }
            },
            (roles ?? []).ToList()
        }
    };
}
