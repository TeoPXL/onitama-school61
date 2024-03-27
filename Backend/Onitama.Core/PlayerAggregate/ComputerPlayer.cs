using System.Drawing;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.Util;

namespace Onitama.Core.PlayerAggregate;

/// <inheritdoc cref="IPlayer"/>
internal class ComputerPlayer
{
    public ComputerPlayer(Color color, Direction direction, IGamePlayStrategy strategy)
    {
    }

    /// <summary>
    /// Uses gameplay strategy to determine the best move to execute.
    /// </summary>
    /// <param name="game">The game (in its current state)</param>
    public IMove DetermineBestMove(IGame game)
    {
        throw new NotImplementedException("TODO: use the strategy to determine the best move");
    }
}