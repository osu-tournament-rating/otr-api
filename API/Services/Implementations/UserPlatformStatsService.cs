using API.DTOs;
using API.Services.Interfaces;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class UserPlatformStatsService(IUserRepository userRepository) : IUserPlatformStatsService
{
    public async Task<UserPlatformStatsDTO> GetAsync() => new()
    {
        SumByDate = await userRepository.GetAccumulatedDailyCountsAsync()
    };
}
