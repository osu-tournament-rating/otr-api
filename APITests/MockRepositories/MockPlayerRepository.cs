using API.Repositories.Interfaces;
using Database.Entities;
using Moq;

namespace APITests.MockRepositories;

public class MockPlayerRepository : Mock<IApiPlayersRepository>
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

    /// <summary>
    /// Configures the repository to mock IPlayerRepository.GetAsync(int id).
    /// This method will return random data regardless of the id passed into
    /// GetAsync.
    /// </summary>
    /// <returns></returns>
    public MockPlayerRepository SetupGet()
    {
        Setup(x => x.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(new Player
            {
                Id = Random.Shared.Next(),
                OsuId = Random.Shared.Next(),
                Created = default,
                Updated = null,
                Username = "RandomPlayer" + Random.Shared.Next(),
                Country = "US"
            });

        return this;
    }
}
