using System.Drawing;
using Onitama.Core.MoveCardAggregate.Contracts;

namespace Onitama.Core.MoveCardAggregate;

/// <inheritdoc cref="IMoveCardFactory"/>
internal class MoveCardFactory : IMoveCardFactory
{
    private Random _random = new Random();
    public IMoveCard Create(string name, MoveCardGridCellType[,] grid, Color[] possibleStampColors)
    {
        int number = _random.Next(0, possibleStampColors.Length);
        var color = possibleStampColors[number];
        return new MoveCard(name, grid, color);
    }
}