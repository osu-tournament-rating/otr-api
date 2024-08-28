using Database.Enums;

namespace Database.Entities.Interfaces;

/// <summary>
/// Interfaces a record of roster information for a <see cref="Match"/> or <see cref="Game"/>
/// </summary>
public interface IWinRecord
{
    /// <summary>
    /// List of <see cref="Player"/> ids on the winning team
    /// </summary>
    public int[] WinnerRoster { get; }

    /// <summary>
    /// List of <see cref="Player"/> ids on the losing team
    /// </summary>
    public int[] LoserRoster { get; }

    /// <summary>
    /// The <see cref="Enums.Team"/> that won
    /// </summary>
    public Team WinnerTeam { get; }

    /// <summary>
    /// The <see cref="Enums.Team"/> that lost
    /// </summary>
    public Team LoserTeam { get; }

    public int WinnerScore { get; }

    public int LoserScore { get; }
}
