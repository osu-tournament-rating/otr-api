using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace API.Configurations;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class ConnectionStringsConfiguration
{
    public const string Position = "ConnectionStrings";

    [Required(ErrorMessage = "DefaultConnection is required!")]
    public string DefaultConnection { get; init; } = string.Empty;

    [Required(ErrorMessage = "CollectorConnection is required!")]
    public string CollectorConnection { get; init; } = string.Empty;

    [Required(ErrorMessage = "RedisConnection is required!")]
    public string RedisConnection { get; init; } = string.Empty;

    [Required(ErrorMessage = "LokiConnection is required!")]
    public string LokiConnection { get; init; } = string.Empty;
}
