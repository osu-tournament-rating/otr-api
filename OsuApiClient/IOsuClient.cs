using OsuApiClient.Net.Authorization;

namespace OsuApiClient;

/// <summary>
/// Interfaces a client that communicates with the osu! API
/// </summary>
public interface IOsuClient : IDisposable
{
    /// <summary>
    /// Updates the current access credentials for the client
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Returns the current credentials if they are not revoked or expired,
    /// returns null if update was unsuccessful
    /// </returns>
    Task<OsuCredentials?> UpdateCredentialsAsync(CancellationToken cancellationToken = default);
}
