using System.ComponentModel.DataAnnotations;

namespace API.Configurations
{
    public class JwtConfiguration
    {
        public const string Position = "Jwt";

        [Required(ErrorMessage = "Key is required!")]
        public string Key { get; set; } = string.Empty;

        [Required(ErrorMessage = "Audience is required!")]
        [Url]
        public string Audience = string.Empty;
    }
}
