using System.ComponentModel.DataAnnotations;

namespace API.Configurations;

public class ConnectionStringsConfiguration
{
    public const string Position = "ConnectionStrings";

    [Required(ErrorMessage = "DefaultConnection is required!")]
    public string DefaultConnection { get; set; } = string.Empty;

    [Required(ErrorMessage = "CollectorConnection is required!")]
    public string CollectorConnection { get; set; } = string.Empty;
}
