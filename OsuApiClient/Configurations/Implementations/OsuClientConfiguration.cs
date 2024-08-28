using OsuApiClient.Configurations.Interfaces;

namespace OsuApiClient.Configurations.Implementations;

public sealed class OsuClientConfiguration : IOsuClientConfiguration
{
    public long ClientId { get; set; }

    public string ClientSecret { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;
}
