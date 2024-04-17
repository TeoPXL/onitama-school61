using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.GameAggregate.Contracts;

public interface IMove
{
    IMoveCard Card { get; }

    /// <summary>
    /// The pawn that is moved.
    /// The pawn can be null when the move only consists of a card swap (because there are no pawns that can be moved).
    /// </summary>
    IPawn Pawn { get; }

    /// <summary>
    /// The direction in which the player (the owner of the pawn) is playing
    /// </summary>
    Direction PlayerDirection { get; }

    /// <summary>
    /// The coordinate the pawn is moved to.
    /// The coordinate can be null when the move only consists of a card swap (because there are no pawns that can be moved).
    /// </summary>
    ICoordinate To { get; }
}