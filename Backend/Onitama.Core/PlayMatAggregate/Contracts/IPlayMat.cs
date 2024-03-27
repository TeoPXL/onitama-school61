using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;

namespace Onitama.Core.PlayMatAggregate.Contracts;


/// <summary>
/// Represents the play mat of the game.
/// </summary>
public interface IPlayMat
{
    /// <summary>
    /// The square grid of the play mat.
    /// </summary>
    IPawn[,] Grid { get; }

    /// <summary>
    /// The size of the play mat (e.g. 5 for a 5x5 grid).
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Positions the school of the player on the play mat.
    /// The direction of the player is used to determine the position of the school
    /// (e.g. when the player is playing north, the school is positioned in the bottom row of the <see cref="Grid"/>).
    /// The master is positioned in the middle of the row and the students are positioned on the left and right of the master.
    /// </summary>
    /// <param name="player">The player whose school needs to be positioned on the mat</param>
    void PositionSchoolOfPlayer(IPlayer player);

    /// <summary>
    /// Retrieves the valid moves for a pawn when playing a specific move card in a specific direction.
    /// From all the possible target coordinates specified by the move card,
    /// only the moves that are not out of bounds and do not capture a friendly pawn are returned.
    /// </summary>
    /// <param name="pawn">The pawn that will possibly be moved</param>
    /// <param name="card">The move card the pawn will use</param>
    /// <param name="playerDirection">The direction in which the owner of the pawn is playing</param>
    IReadOnlyList<IMove> GetValidMoves(IPawn pawn, IMoveCard card, Direction playerDirection);

    /// <summary>
    /// Executes a move on the play mat, but only if the move is valid.
    /// </summary>
    /// <param name="move">The move to execute</param>
    /// <param name="capturedPawn">Will contain the captured pawn if a pawn is captured by the move, null otherwise</param>
    /// <remarks>
    /// The execution of the move results in the position of the pawn being updated on the play mat and in the pawn instance itself.
    /// </remarks>
    void ExecuteMove(IMove move, out IPawn capturedPawn);
}