using Onitama.Core.PlayMatAggregate.Contracts;

namespace Onitama.Core.Util.Contracts;

/// <summary>
/// Represents a coordinate on a <see cref="IPlayMat"/>
/// </summary>
/// <remarks>
/// (0,0) is in the bottom left corner of the play mat.
/// </remarks>
public interface ICoordinate
{
    /// <summary>
    /// The index of the rom on the play mat.
    /// The bottom row has index 0. The top row has index <see cref="IPlayMat.Size"/> - 1"/>
    /// </summary>
    int Row { get; }

    /// <summary>
    /// The index of the column on the play mat.
    /// The leftmost column has index 0. The rightmost column has index <see cref="IPlayMat.Size"/> - 1"/>
    /// </summary>
    int Column { get; }

    /// <summary>
    /// Returns true if the coordinate is outside the play mat
    /// </summary>
    /// <param name="playMatSize">The size of the play mat</param>
    bool IsOutOfBounds(int playMatSize);

    /// <summary>
    /// Returns the neighbor of this coordinate in the specified direction
    /// </summary>
    /// <param name="direction">The direction to look for the neighbor</param>
    ICoordinate GetNeighbor(Direction direction);

    /// <summary>
    /// Returns a new coordinate that is rotated towards the specified direction around (0,0).
    /// </summary>
    /// <param name="direction">The direction the line between (0,0) and the returned coordinate, should point to</param>
    ICoordinate RotateTowards(Direction direction);

    /// <summary>
    /// EXTRA: Returns the length of the straight line between the two coordinates.
    /// This could be used to determine how many steps a piece has to move to reach the other coordinate.
    /// </summary>
    /// <remarks>This is not needed for the minimal requirements, but could be useful when implementing a computer opponent</remarks>
    int GetDistanceTo(ICoordinate other);
}