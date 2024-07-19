using Database.Enums;

namespace API.DTOs.Processor;

public class ProcessorMatchDTO
{
    public int Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public Ruleset Ruleset { get; set; }

    public ProcessorGameDTO[] Games { get; set; } = [];
}
