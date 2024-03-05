using System.IdentityModel.Tokens.Jwt;

namespace API.Utilities;

public static class HttpContextExtensions
{
    /// <summary>
    /// If the user is properly logged in, returns their id.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>An optional user id</returns>
    public static int? AuthorizedUserIdentity(this HttpContext context)
    {
        string? role = context.User.Claims.FirstOrDefault(x => x.Type == "role")?.Value;

        if (role != "user")
        {
            return null;
        }

        return ParseIdFromIssuer(context);
    }

    public static int? AuthorizedClientIdentity(this HttpContext context)
    {
        string? role = context.User.Claims.FirstOrDefault(x => x.Type == "role")?.Value;

        if (role != "client")
        {
            return null;
        }

        return ParseIdFromIssuer(context);
    }

    private static int? ParseIdFromIssuer(HttpContext context)
    {
        string? id = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iss)?.Value;
        if (id == null)
        {
            return null;
        }

        if (!int.TryParse(id, out int idInt))
        {
            return null;
        }

        return idInt;
    }
}
