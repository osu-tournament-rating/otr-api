using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace API.SwaggerGen;

/// <summary>
/// Contains <see cref="OpenApiSecurityRequirement"/>s used within the API
/// </summary>
public static class SecurityRequirements
{
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
    /// formatted with the given list of claims
    /// </summary>
    public static OpenApiSecurityRequirement OAuthSecurityRequirementFromClaims(IEnumerable<string> claims) => new()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" }
            },
            claims.ToList()
        }
    };
}
