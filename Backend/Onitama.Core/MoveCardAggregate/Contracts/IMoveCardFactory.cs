using System.Drawing;

namespace Onitama.Core.MoveCardAggregate.Contracts;

/// <summary>
/// Creates move cards.
/// </summary>
public interface IMoveCardFactory
{
    /// <summary>
    /// Creates a move card.
    /// </summary>
    /// <param name="name">The name of the card</param>
    /// <param name="grid">The 5x5 grid that determines which moves are allowed for the card</param>
    /// <param name="possibleStampColors">
    /// A list of the color of each player. The stamp of move card get a random color from the possible colors. 
    /// </param>
    IMoveCard Create(string name, MoveCardGridCellType[,] grid, Color[] possibleStampColors);
}