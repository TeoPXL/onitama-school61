using System.Drawing;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.MoveCardAggregate.Contracts;

public interface IMoveCard
{
    /// <summary>
    /// Name of the card.
    /// This is the unique identifier of the card.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The 5x5 grid of the card.
    /// Row 0 and column 0 is the bottom left corner.
    /// </summary>
    MoveCardGridCellType[,] Grid { get; }

    /// <summary>
    /// The color of the stamp on the card.
    /// It should be a color of one of the players in the game.
    /// </summary>
    Color StampColor { get; }

    /// <summary>
    /// Uses the <see cref="Grid"/> of the card
    /// to determine all possible moves (of a pawn) from a start coordinate on the play mat
    /// when playing in a certain direction.
    /// </summary>
    /// <param name="startCoordinate">The start coordinate (of a pawn) from which a move is started</param>
    /// <param name="playDirection">
    /// The direction in which the player is playing. E.g. when playing South, the grid of the card should be interpreted upside down.
    /// </param>
    /// <param name="matSize">
    /// The size of the play mat. This is important to determine if a target coordinate is out of bounds.
    /// </param>
    IReadOnlyList<ICoordinate> GetPossibleTargetCoordinates(ICoordinate startCoordinate, Direction playDirection, int matSize);
}