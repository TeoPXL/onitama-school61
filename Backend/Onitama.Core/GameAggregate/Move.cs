using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.GameAggregate;

internal class Move : IMove
{
    public IMoveCard Card { get; }

    public IPawn Pawn { get; }

    public Direction PlayerDirection { get; }

    public ICoordinate To { get; }

    public Move(IMoveCard card)
    {
    }

    public Move(IMoveCard card, IPawn pawn, Direction playerDirection, ICoordinate to)
    {
    }
}