using System.ComponentModel.DataAnnotations;
using OsuApiClient.Configurations.Interfaces;

namespace API.Configurations;

public class OsuConfiguration : IOsuClientConfiguration
{
    public const string Position = "Osu";

    [Required(ErrorMessage = "ClientId is required!")]
    [Range(1, long.MaxValue, ErrorMessage = "ClientId must be a positive, non-zero value!")]
    public long ClientId { get; set; }

    [Required(ErrorMessage = "ClientSecret is required!")]
    public string ClientSecret { get; set; } = string.Empty;

    [Required(ErrorMessage = "RedirectUrl is required!")]
    public string RedirectUrl { get; set; } = string.Empty;

    public int? OsuRateLimit { get; set; }

    public int? OsuTrackRateLimit { get; set; }
}
