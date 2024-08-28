namespace OsuApiClient.Tests.Tests;

/// <summary>
/// Interfaces an integrated test for the <see cref="OsuClient"/>
/// </summary>
public interface IOsuClientTest
{
    /// <summary>
    /// The name of the test
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Business logic of the integrated test
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful</returns>
    Task<bool> RunAsync(CancellationToken cancellationToken);
}
