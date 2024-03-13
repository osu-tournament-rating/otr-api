namespace API.DTOs;

public class TournamentDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Abbreviation { get; set; } = null!;
    public string ForumUrl { get; set; } = null!;
    public int RankRangeLowerBound { get; set; }
    public int Mode { get; set; }
    public int TeamSize { get; set; }
    public int SubmitterUserId { get; set; }
    public ICollection<MatchDTO> Matches { get; set; } = new List<MatchDTO>();
}
