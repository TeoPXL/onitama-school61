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
        Card = card;
        Pawn = pawn;
        PlayerDirection = playerDirection;
        To = to;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Move other = (Move)obj;
        return Card.Equals(other.Card) &&
               Pawn.Equals(other.Pawn) &&
               PlayerDirection == other.PlayerDirection &&
               To.Equals(other.To);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (Card != null ? Card.GetHashCode() : 0);
            hash = hash * 23 + (Pawn != null ? Pawn.GetHashCode() : 0);
            hash = hash * 23 + PlayerDirection.GetHashCode();
            hash = hash * 23 + (To != null ? To.GetHashCode() : 0);
            return hash;
        }
    }
}