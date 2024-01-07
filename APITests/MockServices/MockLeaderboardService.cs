using API.DTOs;
using API.Services.Interfaces;
using Moq;

namespace APITests.MockServices;

public class MockLeaderboardService : Mock<ILeaderboardService>
{
	public MockLeaderboardService MockGetLeaderboardAsync(LeaderboardDTO leaderboardDTO)
	{
		Setup(x => x.GetLeaderboardAsync(It.IsAny<LeaderboardRequestQueryDTO>(), It.IsAny<int?>()))
			.ReturnsAsync(leaderboardDTO);

		return this;
	}
}