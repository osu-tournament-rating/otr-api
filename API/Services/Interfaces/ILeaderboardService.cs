using API.DTOs;

namespace API.Services.Interfaces;

public interface ILeaderboardService
{
    /// <summary>
    /// Gets a leaderboard from the provided
    /// <see cref="LeaderboardRequestQueryDTO"/>
    /// </summary>
    /// <param name="requestQuery">The request data</param>
    /// <param name="authorizedUserId">The id of the user who is requesting the leaderboard</param>
    /// <returns>A <see cref="LeaderboardDTO"/> containing data for a leaderboard
    /// that aligns with the provided <see cref="LeaderboardRequestQueryDTO"/></returns>
    public Task<LeaderboardDTO> GetLeaderboardAsync(
        LeaderboardRequestQueryDTO requestQuery,
        int? authorizedUserId = null
    );
}
