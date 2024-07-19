using Database.Enums;

namespace API.DTOs.Processor;

public class ProcessorGameDTO
{
    public int Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public Ruleset Ruleset { get; set; }

    public int[] WinnerRoster { get; set; } = [];

    public int[] LoserRoster { get; set; } = [];

    public ProcessorGamePlayerPlacementDTO[] PlayerPlacement { get; set; } = [];
}
