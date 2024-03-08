using API.Enums;

namespace API.Entities.Interfaces;

public interface IHistoryEntity : IEntity
{
    /// <summary>
    /// Id of the original record
    /// </summary>
    public int? ReferenceId { get; set; }
    /// <summary>
    /// The type of action taken on the original record, maps to <see cref="HistoryActionType"/>
    /// </summary>
    public int HistoryAction { get; set; }
    /// <summary>
    /// Date that the original data became available
    /// </summary>
    public DateTime? HistoryStartTime { get; set; }
    /// <summary>
    /// Date that the original data was changed / deleted
    /// </summary>
    public DateTime HistoryEndTime { get; set; }
    /// <summary>
    /// User id of the user that took action on the record
    /// </summary>
    public int? ModifierId { get; set; }
}
