using System.ComponentModel.DataAnnotations;

namespace API.Configurations
{
    public class OsuConfiguration
    {
        public const string Position = "Osu";

        [Required(ErrorMessage = "ApiKey is required!")]
        public string ApiKey { get; set; } = String.Empty;

        [Required(ErrorMessage = "ClientId is required!")]
        [Range(1, long.MaxValue, ErrorMessage = "ClientId is required!")]
        public long ClientId { get; set; } = 0;

        [Required(ErrorMessage = "ClientSecret is required!")]
        public string ClientSecret { get; set; } = String.Empty;
        public bool AutoUpdateUsers { get; set; } = false;
        public bool AllowDataFetching { get; set; } = false;
    }
}
