using System.ComponentModel.DataAnnotations;

namespace DataWorkerService.Configurations;

public class ConnectionStringsConfiguration
{
    public const string Position = "ConnectionStrings";

    [Required(ErrorMessage = "DefaultConnection is required!")]
    public string DefaultConnection { get; set; } = string.Empty;

    [Required(ErrorMessage = "RedisConnection is required!")]
    public string RedisConnection { get; set; } = string.Empty;

}
