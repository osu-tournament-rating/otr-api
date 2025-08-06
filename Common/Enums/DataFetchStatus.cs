namespace Common.Enums;

/// <summary>
/// Represents the status of data fetching from external APIs
/// </summary>
public enum DataFetchStatus
{
    /// <summary>
    /// Data has not been fetched yet
    /// </summary>
    NotFetched = 0,

    /// <summary>
    /// Data is currently being fetched
    /// </summary>
    Fetching = 1,

    /// <summary>
    /// Data was successfully fetched and saved
    /// </summary>
    Fetched = 2,

    /// <summary>
    /// Data could not be found (e.g., API returned null or 404)
    /// </summary>
    NotFound = 3,

    /// <summary>
    /// An error occurred while fetching data
    /// </summary>
    Error = 4
}
