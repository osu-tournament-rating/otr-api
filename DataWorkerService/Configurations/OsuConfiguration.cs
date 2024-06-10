using System.ComponentModel.DataAnnotations;

namespace DataWorkerService.Configurations;

public class OsuConfiguration
{
    public const string Position = "Osu";

    [Required(ErrorMessage = "ApiKey is required!")]
    public string ApiKey { get; set; } = string.Empty;

    [Required(ErrorMessage = "ClientId is required!")]
    [Range(1, long.MaxValue, ErrorMessage = "ClientId must be a positive, non-zero value!")]
    public long ClientId { get; set; }

    [Required(ErrorMessage = "ClientSecret is required!")]
    public string ClientSecret { get; set; } = string.Empty;
    public bool AutoUpdateUsers { get; set; }
    public bool AllowDataFetching { get; set; }
}
