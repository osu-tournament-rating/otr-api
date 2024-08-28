using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OsuApiClient.Net.JsonModels;

/// <summary>
/// Base class for JSON models
/// </summary>
public class JsonModelBase : IJsonModel
{
    /// <summary>
    /// Dictionary containing data from a raw response that was not deserialized to the model
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, JToken> UnmappedData { get; set; } = null!;
}

public interface IJsonModel;
