using API.Entities;
using AutoMapper;
using AutoMapper.Configuration.Annotations;

namespace API.DTOs;

[AutoMap(typeof(MatchScore))]
public class MatchScoreDTO
{
    public int PlayerId { get; set; }
    public int Team { get; set; }
    public long Score { get; set; }
    public int? EnabledMods { get; set; }
    [SourceMember(nameof(MatchScore.CountMiss))]
    public int Misses { get; set; }
    public double AccuracyStandard { get; set; }
    public double AccuracyTaiko { get; set; }
    public double AccuracyCatch { get; set; }
    public double AccuracyMania { get; set; }
}
