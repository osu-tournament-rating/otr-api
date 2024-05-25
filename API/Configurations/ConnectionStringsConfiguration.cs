using System.ComponentModel.DataAnnotations;
using Database.Configurations;

namespace API.Configurations;

public class ConnectionStringsConfiguration : IConnectionStringsConfiguration
{
    public const string Position = "ConnectionStrings";

    [Required(ErrorMessage = "DefaultConnection is required!")]
    public string DefaultConnection { get; set; } = string.Empty;

    [Required(ErrorMessage = "CollectorConnection is required!")]
    public string CollectorConnection { get; set; } = string.Empty;

    [Required(ErrorMessage = "RedisConnection is required!")]
    public string RedisConnection { get; set; } = string.Empty;
}
