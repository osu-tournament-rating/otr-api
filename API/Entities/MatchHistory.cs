using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using API.Enums;

namespace API.Entities;

[Table("matches_hist")]
public class MatchHistory : MatchEntityBase
{
    /// <summary>
    /// Id of the original record
    /// </summary>
    [Column("hist_ref_id")]
    public int? ReferenceId { get; set; }
    /// <summary>
    /// The type of action taken on the original record, maps to <see cref="HistoryActionType"/>
    /// </summary>
    [Column("hist_action")]
    public int HistoryAction { get; set; }
    /// <summary>
    /// Date that the original data became available
    /// </summary>
    [Column("hist_start_time", TypeName = "timestamp with time zone")]
    public DateTime? HistoryStartTime { get; set; }
    /// <summary>
    /// Date that the original data was changed / deleted
    /// </summary>
    [Column("hist_end_time", TypeName = "timestamp with time zone")]
    public DateTime HistoryEndTime { get; set; }
    /// <summary>
    /// User id of the user that took action on the record
    /// </summary>
    [Column("hist_modifier_id")]
    public int? ModifierId { get; set; }
    /// <summary>
    /// The current Match record
    /// </summary>
    public virtual Match? ReferenceMatch { get; set; }
}
