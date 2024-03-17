// ReSharper disable CommentTypo

using System.Diagnostics.CodeAnalysis;

namespace API.DTOs;

public class TournamentDTO
{
    public string Name { get; set; } = null!;
    public string Abbreviation { get; set; } = null!;
    public string ForumUrl { get; set; } = null!;
    public int RankRangeLowerBound { get; set; }
    public int Mode { get; set; }
    public int TeamSize { get; set; }
    // Requested by Cytusine, normally we don't return this info.
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public DateTime Created { get; set; }
}
