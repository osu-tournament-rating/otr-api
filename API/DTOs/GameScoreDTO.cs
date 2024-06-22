using Database.Enums;

namespace API.DTOs;

public class GameScoreDTO
{
    public int PlayerId { get; set; }
    public Team Team { get; set; }
    public long Score { get; set; }
    public Mods Mods { get; set; }
    public int Misses { get; set; }
    public double AccuracyStandard { get; set; }
    public double AccuracyTaiko { get; set; }
    public double AccuracyCatch { get; set; }
    public double AccuracyMania { get; set; }
}
