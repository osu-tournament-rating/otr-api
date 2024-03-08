using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Interfaces;

namespace API.Entities;

[Table("matches_hist")]
public class MatchHistory : MatchEntityBase, IHistoryEntity
{
    [Column("hist_ref_id")]
    public int? ReferenceId { get; set; }
    [Column("hist_action")]
    public int HistoryAction { get; set; }
    [Column("hist_start_time", TypeName = "timestamp with time zone")]
    public DateTime? HistoryStartTime { get; set; }
    [Column("hist_end_time", TypeName = "timestamp with time zone")]
    public DateTime HistoryEndTime { get; set; }
    [Column("hist_created")]
    public DateTime Created { get; set; }
    [Column("hist_modifier_id")]
    public int? ModifierId { get; set; }
    /// <summary>
    /// The current Match record
    /// </summary>
    public virtual Match? ReferenceMatch { get; set; }
}
