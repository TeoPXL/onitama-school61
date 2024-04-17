using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.UserAggregate;

namespace Onitama.Core.TableAggregate.Contracts;

/// <summary>
/// A (virtual) table where players can join and play a game.
/// </summary>
public interface ITable
{
    /// <summary>
    /// The unique identifier of the table.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The preferences of the table. This determines game play options like the number of players and the play mat size.
    /// </summary>
    TablePreferences Preferences { get; }

    /// <summary>
    /// The unique identifier of the player who created the table.
    /// </summary>
    Guid OwnerPlayerId { get; }

    /// <summary>
    /// The players who are currently seated at the table.
    /// </summary>
    public IReadOnlyList<IPlayer> SeatedPlayers { get; }

    /// <summary>
    /// Indicates whether there is an available seat at the table.
    /// </summary>
    public bool HasAvailableSeat { get; }

    /// <summary>
    /// The unique identifier of the game that is being played at the table.
    /// When no game is started yet, this property is set to <see cref="Guid.Empty"/>.
    /// </summary>
    Guid GameId { get; set; }

    /// <summary>
    /// Adds a player to the table.
    /// </summary>
    /// <param name="user">User for which a player should be created</param>
    void Join(User user);

    /// <summary>
    /// Removes a player from the table.
    /// </summary>
    /// <param name="userId">Unique identifier of the user (player) that wants to leave the table</param>
    void Leave(Guid userId);

    /// <summary>
    /// EXTRA: Fills the table with computer players.
    /// </summary>
    /// <param name="gamePlayStrategy">Strategy that should be used by the AI players to determine their next move</param>
    /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
    void FillWithArtificialPlayers(IGamePlayStrategy gamePlayStrategy);
}