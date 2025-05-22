using System.ComponentModel.DataAnnotations;
using OsuApiClient.Configurations.Interfaces;

namespace OsuApiClient.Configurations.Implementations;

public sealed class OsuClientConfiguration : IOsuClientConfiguration
{
    public const string Position = "OsuClient";

    [Required]
    public long ClientId { get; set; }

    [Required]
    public string ClientSecret { get; set; } = string.Empty;

    [Required]
    public string RedirectUrl { get; set; } = string.Empty;

    public int OsuRateLimit { get; set; } = 60;

    public int OsuTrackRateLimit { get; set; } = 30;

    public bool EnableDistributedLocking { get; set; } = false;
}
