using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents information about a user's kudosu
/// </summary>
[AutoMap(typeof(KudosuJsonModel))]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "CommentTypo")]
public class Kudosu : IModel
{
    /// <summary>
    /// Number of available kudosu
    /// </summary>
    public int Available { get; set; }

    /// <summary>
    /// Total number of kudosu
    /// </summary>
    public int Total { get; set; }
}
