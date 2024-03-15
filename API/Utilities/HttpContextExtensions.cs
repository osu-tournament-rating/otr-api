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
        var user = context.User.IsUser();

        return user ? ParseIdFromIssuer(context) : null;
    }

    public static int? AuthorizedClientIdentity(this HttpContext context)
    {
        return !context.User.IsClient() ? null : ParseIdFromIssuer(context);
    }

    private static int? ParseIdFromIssuer(HttpContext context)
    {
        var id = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iss)?.Value;
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
}
