using Microsoft.Extensions.Primitives;
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
		string? id = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;
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

	public static StringValues WebAuthorization(this IHeaderDictionary headers) => headers["WebAuthorization"];
}