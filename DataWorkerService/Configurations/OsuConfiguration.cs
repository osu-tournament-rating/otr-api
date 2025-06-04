using System.ComponentModel.DataAnnotations;
using OsuApiClient.Configurations.Interfaces;

namespace DataWorkerService.Configurations;

/// <summary>
/// Configures the <see cref="OsuApiClient.OsuClient"/>
/// </summary>
public class OsuConfiguration : IOsuClientConfiguration
{
    public const string Position = "Osu";

    [Required(ErrorMessage = "ClientId is required!")]
    public long ClientId { get; set; }

    [Required(ErrorMessage = "ClientSecret is required!")]
    public string ClientSecret { get; set; } = string.Empty;

    // This value is not used in the DataWorkerService
    public string RedirectUrl { get; set; } = string.Empty;

    public int OsuRateLimit { get; set; } = 60;

    public int OsuTrackRateLimit { get; set; } = 30;

    public bool EnableDistributedLocking { get; set; } = true;
}
