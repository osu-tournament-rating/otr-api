using JetBrains.Annotations;

namespace Common.Enums;

/// <summary>
/// Describes the ranked status of a <see cref="Database.Entities.Beatmap"/>
/// </summary>
/// /// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#beatmapset-rank-status
/// Last accessed June 2024
/// </copyright>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public enum BeatmapRankedStatus
{
    Graveyard = -2,

    WorkInProgress = -1,

    Pending = 0,

    Ranked = 1,

    Approved = 2,

    Qualified = 3,

    Loved = 4
}
