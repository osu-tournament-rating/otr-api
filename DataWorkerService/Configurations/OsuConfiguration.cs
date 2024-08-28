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
    public string ClientSecret { get; set; } = null!;

    public string RedirectUrl { get; set; } = string.Empty;
}
