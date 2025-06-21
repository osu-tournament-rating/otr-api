using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Multiplayer;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed June 2024
/// </copyright>
public class GameSlotInfoJsonModel
{
    [JsonProperty("slot")]
    public int Slot { get; set; }

    [JsonProperty("team")]
    public string Team { get; set; } = null!;

    [JsonProperty("pass")]
    public bool Pass { get; set; }
}
