using System.Drawing;
using Onitama.Core.MoveCardAggregate.Contracts;

namespace Onitama.Core.MoveCardAggregate;

/// <inheritdoc cref="IMoveCardFactory"/>
internal class MoveCardFactory : IMoveCardFactory
{
    public IMoveCard Create(string name, MoveCardGridCellType[,] grid, Color[] possibleStampColors)
    {
        throw new NotImplementedException("TODO: pick a random stamp color and return a MoveCard instance");
    }
}