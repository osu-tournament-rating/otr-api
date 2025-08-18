using System.ComponentModel.DataAnnotations;
using OsuApiClient.Configurations.Interfaces;

namespace DWS.Configurations;

public class OsuConfiguration : IOsuClientConfiguration
{
    public const string Position = "Osu";

    [Required(ErrorMessage = "ClientId is required!")]
    [Range(1, long.MaxValue, ErrorMessage = "ClientId must be a positive, non-zero value!")]
    public long ClientId { get; set; }

    [Required(ErrorMessage = "ClientSecret is required!")]
    public string ClientSecret { get; set; } = string.Empty;

    // This value is not used in the DWS but required by the interface
    public string RedirectUrl { get; set; } = "http://localhost:5075/api/v1/auth/callback";

    public int OsuRateLimit { get; set; } = 60;

    public int OsuTrackRateLimit { get; set; } = 30;
}
