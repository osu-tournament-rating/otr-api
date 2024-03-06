namespace API.Entities.Interfaces;

public interface IHistoryEntity
{
    public int? ReferenceId { get; set; }
    public int HistoryAction { get; set; }
    public DateTime? HistoryStartTime { get; set; }
    public DateTime HistoryEndTime { get; set; }
    public int? ModifierId { get; set; }
}
