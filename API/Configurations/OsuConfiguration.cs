using System.ComponentModel.DataAnnotations;

namespace API.Configurations;

public class OsuConfiguration
{
    public const string Position = "Osu";

    [Required(ErrorMessage = "ClientId is required!")]
    [Range(1, long.MaxValue, ErrorMessage = "ClientId must be a positive, non-zero value!")]
    public long ClientId { get; init; }

    [Required(ErrorMessage = "ClientSecret is required!")]
    public string ClientSecret { get; init; } = string.Empty;
}
