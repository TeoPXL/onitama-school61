using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util.Contracts;
using System;

namespace Onitama.Core.GameAggregate.Contracts
{
    public interface IGame
    {
        /// <summary>
        /// The unique identifier of the game
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The play mat
        /// </summary>
        IPlayMat PlayMat { get; }

        /// <summary>
        /// The one card (of the 5) that is not with any player
        /// </summary>
        IMoveCard ExtraMoveCard { get; }

        /// <summary>
        /// The players. In a normal game there are 2 players.
        /// </summary>
        IPlayer[] Players { get; }

        /// <summary>
        /// The unique identifier of the player who's turn it is
        /// </summary>
        Guid PlayerToPlayId { get; }

        /// <summary>
        /// The unique identifier of the player that has won the game.
        /// If no player has won yet, this will be an empty Guid (00000000-0000-0000-0000-000000000000).
        /// </summary>
        public Guid WinnerPlayerId { get; }

        /// <summary>
        /// Indicates how the game was won. 'By way of the stone' or 'by way of the wind'.
        /// If no player has won yet, this is an empty string. 
        /// </summary>
        public string WinnerMethod { get; }

        /// <summary>
        /// Returns all the moves a player can make for a specific pawn and move card.
        /// </summary>
        /// <param name="playerId">Unique identifier of the player</param>
        /// <param name="pawnId">Unique identifier of the pawn</param>
        /// <param name="moveCardName">Name of the move card (e.g. "Dragon")</param>
        IReadOnlyList<IMove> GetPossibleMovesForPawn(Guid playerId, Guid pawnId, string moveCardName);

        /// <summary>
        /// Returns all the moves a player can make for all his pawns and his two move cards.
        /// </summary>
        /// <param name="playerId">Unique identifier of the player</param>
        IReadOnlyList<IMove> GetAllPossibleMovesFor(Guid playerId);

        /// <summary>
        /// Moves a pawn on the play mat.
        /// </summary>
        /// <param name="playerId">Unique identifier of the player that is making the move</param>
        /// <param name="pawnId">Unique identifier of the pawn</param>
        /// <param name="moveCardName">Name of the move card (e.g. "Mantis")</param>
        /// <param name="to">Target coordinate on the play mat</param>
        void MovePawn(Guid playerId, Guid pawnId, string moveCardName, ICoordinate to);

        /// <summary>
        /// When a player cannot move any of his pawns, he can skip his movement,
        /// but he must exchange one of his move cards.
        /// </summary>
        /// <param name="playerId">Unique identifier of the player</param>
        /// <param name="moveCardName">Name of the move card (e.g. "Boar")</param>
        void SkipMovementAndExchangeCard(Guid playerId, string moveCardName);

        /// <summary>
        /// Returns the player who's turn it is after a certain player.
        /// For a 2 player game, this will be the other player.
        /// For a 4 player game, this will be the next player in the circle (EXTRA).
        /// </summary>
        /// <param name="playerId">Unique identifier of the player</param>
        IPlayer GetNextOpponent(Guid playerId);
    }
}
