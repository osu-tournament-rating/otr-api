using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Interfaces;
using AutoMapper;
using AutoMapper.Configuration.Annotations;

namespace API.Entities;

[AutoMap(typeof(Match))]
[Table("matches_hist")]
public class MatchHistory : MatchEntityBase, IHistoryEntity
{
    [SourceMember(nameof(Match.Id))]
    [Column("hist_ref_id")]
    public int? ReferenceId { get; set; }

    [Ignore]
    [Column("hist_action")]
    public int HistoryAction { get; set; }

    [SourceMember(nameof(Match.Updated))]
    [Column("hist_start_time", TypeName = "timestamp with time zone")]
    public DateTime? HistoryStartTime { get; set; }

    [Ignore]
    [Column("hist_end_time", TypeName = "timestamp with time zone")]
    public DateTime HistoryEndTime { get; set; }

    [Ignore]
    [Column("hist_created")]
    public DateTime Created { get; set; }

    [Ignore]
    [Column("hist_modifier_id")]
    public int? ModifierId { get; set; }

    /// <summary>
    /// The current Match record
    /// </summary>
    [Ignore]
    public virtual Match? ReferenceMatch { get; set; }
}
