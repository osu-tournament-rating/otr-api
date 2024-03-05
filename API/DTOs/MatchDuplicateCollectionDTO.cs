// ReSharper disable CollectionNeverQueried.Global

namespace API.DTOs;

public class MatchDuplicateCollectionDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long OsuMatchId { get; set; }
    public IList<MatchDuplicateDTO> SuspectedDuplicates { get; set; } = new List<MatchDuplicateDTO>();
}
