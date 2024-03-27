using System.Drawing;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;

namespace Onitama.Core.PlayerAggregate.Contracts;

/// <summary>
/// Represents a player in the game.
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// Unique identifier of the player
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// (Display) name of the player
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Color of the player
    /// </summary>
    Color Color { get; }

    /// <summary>
    /// Direction in which the player is playing (north or south)
    /// </summary>
    Direction Direction { get; }

    /// <summary>
    /// The school of the player (1 master and 4 students)
    /// </summary>
    ISchool School { get; }

    /// <summary>
    /// The move cards that the player can use to play its next move (2 of the 5 available)
    /// </summary>
    IList<IMoveCard> MoveCards { get; }
}