using Database.Entities.Interfaces;
using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Multiplayer;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed June 2024
/// </copyright>
public class GameScoreStatisticsJsonModel : JsonModelBase, IScoreStatistics
{
    [JsonProperty("count_300")]
    public int Count300 { get; set; }

    [JsonProperty("count_100")]
    public int Count100 { get; set; }

    [JsonProperty("count_50")]
    public int Count50 { get; set; }

    [JsonProperty("count_geki")]
    public int CountGeki { get; set; }

    [JsonProperty("count_katu")]
    public int CountKatu { get; set; }

    [JsonProperty("count_miss")]
    public int CountMiss { get; set; }
}
