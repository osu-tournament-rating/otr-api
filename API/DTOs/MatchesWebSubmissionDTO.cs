namespace API.DTOs;

public class MatchesWebSubmissionDTO
{
    public int SubmitterId { get; set; }
    public IEnumerable<long> Ids { get; set; } = [];
}
