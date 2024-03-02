using API.Utilities;
using System.Security.Claims;

namespace APITests.Utilities;

public class ClaimsPrincipalExtensionsTests
{
	[Fact]
	public void ClaimsPrincipal_Default_HasNoSpecialPermissions()
	{
		var claims = new ClaimsPrincipal();
		Assert.False(claims.IsAdmin());
		Assert.False(claims.IsSystem());
		Assert.False(claims.IsMatchVerifier());
	}
	
	[Fact]
	public void ClaimsPrincipal_IsAdmin()
	{
		var claims = new ClaimsPrincipal();
		claims.AddIdentity(new ClaimsIdentity(new List<Claim>
		{
			new(ClaimTypes.Role, "admin")
		}));

		Assert.True(claims.IsAdmin());
	}

	[Fact]
	public void ClaimsPrincipal_IsMatchVerifier()
	{
		var claims = new ClaimsPrincipal();
		claims.AddIdentity(new ClaimsIdentity(new List<Claim>
		{
			new(ClaimTypes.Role, "verifier")
		}));

		Assert.True(claims.IsMatchVerifier());
		Assert.False(claims.IsAdmin());
		Assert.False(claims.IsSystem());
	}

	[Fact]
	public void ClaimsPrincipal_Admin_IsMatchVerifier()
	{
		var claims = new ClaimsPrincipal();
		claims.AddIdentity(new ClaimsIdentity(new List<Claim>
		{
			new(ClaimTypes.Role, "admin")
		}));
		
		Assert.True(claims.IsAdmin());
	}
	
	[Fact]
	public void ClaimsPrincipal_System_IsAdmin()
	{
		var claims = new ClaimsPrincipal();
		claims.AddIdentity(new ClaimsIdentity(new List<Claim>
		{
			new(ClaimTypes.Role, "system")
		}));

		Assert.True(claims.IsAdmin());
		Assert.True(claims.IsSystem());
	}
}