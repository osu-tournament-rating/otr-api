using System.ComponentModel.DataAnnotations;

namespace API.Configurations;

public class AuthConfiguration
{
    public const string Position = "Auth";

    [Required(ErrorMessage = "ClientCallbackUrl is required!")]
    public string ClientCallbackUrl { get; set; } = string.Empty;

    public bool EnforceWhitelist { get; set; }
}
