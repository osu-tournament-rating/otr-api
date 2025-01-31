using OsuApiClient.Configurations.Interfaces;

namespace API.Configurations.Interfaces;

public interface IOsuConfiguration : IOsuClientConfiguration
{
    /// <summary>
    ///     The minimum amount of hours to wait before
    ///     syncing a user's friends list automatically via logging in
    /// </summary>
    int LoginFriendsSyncFrequencyHours { get; set; }
}
