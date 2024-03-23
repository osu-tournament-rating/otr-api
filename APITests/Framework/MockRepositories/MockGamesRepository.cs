using API.Entities;
using API.Repositories.Interfaces;
using APITests.SeedData;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockGamesRepository : Mock<IGamesRepository>
{
    public MockGamesRepository SetupGet()
    {
        Setup(x =>
                x.GetAsync(It.IsAny<int>())
            )
            .ReturnsAsync((int id) =>
            {
                Game game = SeededGame.Get();

                return game;
            });

        return this;
    }
}
