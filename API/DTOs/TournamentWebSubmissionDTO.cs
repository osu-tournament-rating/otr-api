namespace API.DTOs;

public class TournamentWebSubmissionDTO
{
    public string TournamentName { get; set; } = null!;
    public string Abbreviation { get; set; } = null!;
    public string ForumPost { get; set; } = null!;
    public int RankRangeLowerBound { get; set; }
    public int TeamSize { get; set; }
    public int Mode { get; set; }
    public int SubmitterId { get; set; }

    /// <summary>
    /// List of tournament match ids.
    /// </summary>
    public IEnumerable<long> Ids { get; set; } = new List<long>();
}
