using System.ComponentModel.DataAnnotations;

namespace DWS.Configurations;

/// <summary>
/// Configuration for connection strings used by DWS
/// </summary>
public class ConnectionStringsConfiguration
{
    public const string Position = "ConnectionStrings";

    /// <summary>
    /// PostgreSQL database connection string
    /// </summary>
    [Required(ErrorMessage = "DefaultConnection is required!")]
    public string DefaultConnection { get; init; } = string.Empty;

    /// <summary>
    /// Redis cache connection string
    /// </summary>
    [Required(ErrorMessage = "RedisConnection is required!")]
    public string RedisConnection { get; init; } = string.Empty;
}
