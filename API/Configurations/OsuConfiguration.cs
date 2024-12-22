using System.ComponentModel.DataAnnotations;
using API.Configurations.Interfaces;

namespace API.Configurations;

public class OsuConfiguration : IOsuConfiguration
{
    public const string Position = "Osu";

    [Required(ErrorMessage = "ClientId is required!")]
    [Range(1, long.MaxValue, ErrorMessage = "ClientId must be a positive, non-zero value!")]
    public long ClientId { get; set; }

    [Required(ErrorMessage = "ClientSecret is required!")]
    public string ClientSecret { get; set; } = string.Empty;

    [Required(ErrorMessage = "RedirectUrl is required!")]
    public string RedirectUrl { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "LoginFriendsSyncFrequencyHours must be a positive, non-zero value!")]
    public int LoginFriendsSyncFrequencyHours { get; set; } = 24;

    [Range(1, 1200, ErrorMessage = "OsuRateLimit must be between 1 and 1200!")]
    public int? OsuRateLimit { get; set; }

    [Range(1, 300, ErrorMessage = "OsuRateLimit must be between 1 and 300!")]
    public int? OsuTrackRateLimit { get; set; }
}
