using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace DataWorkerService.Configurations;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class ConnectionStringsConfiguration
{
    public const string Position = "ConnectionStrings";

    [Required(ErrorMessage = "DefaultConnection is required!")]
    public string DefaultConnection { get; set; } = string.Empty;

    [Required(ErrorMessage = "RedisConnection is required!")]
    public string RedisConnection { get; set; } = string.Empty;

}
