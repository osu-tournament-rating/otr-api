using Newtonsoft.Json;
using OsuApiClient.Net.JsonModels.Osu.Users;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Net.JsonModels.Osu.Multiplayer;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed June 2024
/// </copyright>
public class MatchUserJsonModel : UserJsonModel
{
    [JsonProperty("profile_colour")]
    public string? ProfileColour { get; set; }

    [JsonProperty("country")]
    public CountryJsonModel Country { get; set; } = null!;
}
