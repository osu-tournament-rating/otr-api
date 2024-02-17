using API.Repositories.Interfaces;
using APITests.SeedData;
using Moq;

namespace APITests.MockRepositories;

public class MockPlayerRepository : Mock<IPlayerRepository>
{
	public MockPlayerRepository SetupGetId()
	{
		Setup(x => x.GetIdAsync(It.IsAny<string>())).ReturnsAsync(440);

		return this;
	}

	public MockPlayerRepository SetupGetOsuId()
	{
		Setup(x => x.GetOsuIdAsync(It.IsAny<int>())).ReturnsAsync(123456);

		return this;
	}

	public MockPlayerRepository SetupGetUsername()
	{
		Setup(x => x.GetUsernameAsync(It.IsAny<long>())).ReturnsAsync("FooBar");

		return this;
	}

	public MockPlayerRepository SetupGetCountry()
	{
		Setup(x => x.GetCountryAsync(It.IsAny<int>())).ReturnsAsync("US");

		return this;
	}
}