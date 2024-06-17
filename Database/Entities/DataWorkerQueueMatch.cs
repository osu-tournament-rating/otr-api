using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

/// <summary>
/// A future <see cref="Match"/> that is awaiting processing by the Data Worker
/// </summary>
[Table("data_worker_queue_matches")]
public class DataWorkerQueueMatch : EntityBase
{
    /// <summary>
    /// osu! id
    /// </summary>
    [Column("osu_match_id")]
    public long OsuMatchId { get; init; }

    /// <summary>
    /// Tournament id
    /// </summary>
    [Column("tournament_id")]
    public long TournamentId { get; init; }
}
