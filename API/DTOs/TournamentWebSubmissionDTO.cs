using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace API.DTOs;

/// <summary>
/// Represents data used to create a tournament
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class TournamentWebSubmissionDTO : MatchesWebSubmissionDTO
{
    /// <inheritdoc cref="TournamentDTO.Name"/>
    [Required]
    public string TournamentName { get; set; } = null!;

    /// <inheritdoc cref="TournamentDTO.Abbreviation"/>
    [Required]
    public string Abbreviation { get; set; } = null!;

    /// <inheritdoc cref="TournamentDTO.ForumUrl"/>
    [Required]
    public string ForumPost { get; set; } = null!;

    /// <inheritdoc cref="TournamentDTO.RankRangeLowerBound"/>
    [Required]
    public int RankRangeLowerBound { get; set; }

    /// <inheritdoc cref="TournamentDTO.TeamSize"/>
    [Required]
    public int TeamSize { get; set; }

    /// <inheritdoc cref="TournamentDTO.Mode"/>
    [Required]
    public int Mode { get; set; }
}
