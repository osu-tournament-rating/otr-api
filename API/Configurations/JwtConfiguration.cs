using System.ComponentModel.DataAnnotations;
using API.Utilities;

namespace API.Configurations;

public class JwtConfiguration
{
    public const string Position = "Jwt";

    [BitLength(Minimum = 128, ErrorMessage = "Key must be at least 128 bits!")]
    [Required(ErrorMessage = "Key is required!")]
    public string Key { get; init; } = string.Empty;

    [Required(ErrorMessage = "Audience is required!")]
    public string Audience { get; init; } = string.Empty;
}
