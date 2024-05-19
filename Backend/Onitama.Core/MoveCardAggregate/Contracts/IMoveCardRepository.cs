using System.Drawing;

namespace Onitama.Core.MoveCardAggregate.Contracts;

public interface IMoveCardRepository
{
    IMoveCard[] LoadSet(MoveCardSet set, Color[] possibleStampColors);
    IMoveCard[] LoadSetCustom(MoveCardSet set, Color[] possibleStampColors, string customJson = null);
}