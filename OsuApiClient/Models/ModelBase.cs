using AutoMapper;
using Newtonsoft.Json.Linq;
using OsuApiClient.Net.JsonModels;

namespace OsuApiClient.Models;

/// <summary>
/// Base class for models
/// </summary>
[AutoMap(typeof(JsonModelBase))]
public class ModelBase : IModel
{
    /// <summary>
    /// Dictionary containing data from a raw response that was not deserialized to the model
    /// </summary>
    public IDictionary<string, JToken> UnmappedData { get; set; } = null!;
}

public interface IModel;
