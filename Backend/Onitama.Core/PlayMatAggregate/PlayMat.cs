using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using System.Data.Common;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;

namespace Onitama.Core.PlayMatAggregate;

/// <inheritdoc cref="IPlayMat"/>
internal class PlayMat 
{
    /// <summary>
    /// Creates a play mat that is a copy of another play mat
    /// </summary>
    /// <param name="otherPlayMat">The play mat to copy</param>
    /// <param name="copiedPlayers">
    /// Copies of the players (with their school)
    /// that should be used to position pawn on the copy of the <paramref name="otherPlayMat"/>.</param>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public PlayMat(IPlayMat otherPlayMat, IPlayer[] copiedPlayers)
    {
        throw new NotImplementedException("TODO: copy properties of other playmat");
    }
}