using API.Services.Implementations;
using API.Services.Interfaces;
using Database.Repositories.Interfaces;
using Moq;

namespace APITests.Services;

public class TournamentsServiceTests : TestBase
{
    private readonly Mock<ITournamentsRepository> _mockTournamentsRepo;
    private readonly Mock<IMatchesRepository> _mockMatchesRepo;
    private readonly Mock<IBeatmapsRepository> _mockBeatmapsRepo;

    private readonly ITournamentsService _service;

    public TournamentsServiceTests()
    {
        _mockTournamentsRepo = new Mock<ITournamentsRepository>();
        _mockMatchesRepo = new Mock<IMatchesRepository>();
        _mockBeatmapsRepo = new Mock<IBeatmapsRepository>();

        _service = new TournamentsService(_mockTournamentsRepo.Object,
            _mockMatchesRepo.Object,
            _mockBeatmapsRepo.Object,
            GetMapper());
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
    {
        // Arrange
        const int id = 5;

        _mockTournamentsRepo.Setup(x => x.ExistsAsync(id))
            .ReturnsAsync(true);

        // Act
        var result = await _service.ExistsAsync(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Arrange
        const int id = 5;

        _mockTournamentsRepo.Setup(x => x.ExistsAsync(id))
            .ReturnsAsync(false);

        // Act
        var result = await _service.ExistsAsync(id);

        // Assert
        Assert.False(result);
    }
}
