using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

/// <summary>
/// Represents data used to add matches to an existing tournament
/// </summary>
public class MatchesWebSubmissionDTO
{
    /// <summary>
    /// The id of the submitting User
    /// </summary>
    [Range(1, int.MaxValue)]
    public int SubmitterId { get; set; }

    /// <summary>
    /// List of osu! match ids
    /// </summary>
    /// <example>For a match link https://osu.ppy.sh/mp/98119977, add 98119977 to this list</example>
    public IEnumerable<long> Ids { get; set; } = new List<long>();
}
