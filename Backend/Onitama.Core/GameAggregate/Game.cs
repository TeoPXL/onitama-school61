using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using System.Linq;
using System.Linq.Expressions;

namespace Onitama.Core.GameAggregate;

/// <inheritdoc cref="IGame"/>
internal class Game : IGame
{
    private Guid _id;
    private IPlayMat _playMat;
    private IMoveCard _extraMoveCard;
    private IPlayer[] _players;
    private Guid _playerToPlayId;
    private Guid _winnerPlayerId;


    public Guid Id
    {
        get { return _id; }
        set {  _id = value; }
    }

    public IPlayMat PlayMat
    {
        get { return _playMat; }
        set { this._playMat = value; }
    }


    public IMoveCard ExtraMoveCard
    {
        get { return _extraMoveCard; }
        set { this._extraMoveCard = value; }
    }

    public IPlayer[] Players
    {
        get { return _players; }
        set { this._players = value; }
    }

    public Guid PlayerToPlayId
    {
        get { return _playerToPlayId; }
        set { this._playerToPlayId = value; }
    }

    public Guid WinnerPlayerId
    {
        get { return _winnerPlayerId; }
        set { this._winnerPlayerId = value; }
    }

    public string WinnerMethod => throw new NotImplementedException();

    /// <summary>
    /// Creates a new game and determines the player to play first.
    /// </summary>
    /// <param name="id">The unique identifier of the game</param>
    /// <param name="playMat">
    /// The play mat
    /// (with the schools of the player already positioned on it)
    /// </param>
    /// <param name="players">
    /// The 2 players that will play the game
    /// (with 2 move cards each)
    /// </param>
    /// <param name="extraMoveCard">
    /// The fifth card used to exchange cards after the first move
    /// </param>
    public Game(Guid id, IPlayMat playMat, IPlayer[] players, IMoveCard extraMoveCard)
    {
        this._players = new IPlayer[players.Count()];
        //throw new Exception(players[0].ToString());
        for (int i = 0; i < _players.Length; i++)
        {
            _players[i] = players.ElementAt(i);
        }
        //this._players = players;
        this._id = id;  
        this._playMat = playMat;
        this._extraMoveCard = extraMoveCard;
    }

    /// <summary>
    /// Creates a game that is a copy of another game.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public Game(IGame otherGame)
    {
        this._id = otherGame.Id;
        this._playMat = otherGame.PlayMat;
        foreach(var element in otherGame.Players)
        {
            this._players.Append(element);
        }
        //Attention: the players should be copied, not just referenced
    }

    public IReadOnlyList<IMove> GetPossibleMovesForPawn(Guid playerId, Guid pawnId, string moveCardName)
    {
        IPlayer player = null;
        IPawn pawn = null;
        IMoveCard moveCard = null;

        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].Id == playerId)
            {
                player = Players[i];
            }
        }

        if (player == null)
        {
            throw new InvalidOperationException("There is no player with that ID");
        }

        for (int i = 0; i < player.School.AllPawns.Length; i++)
        {
            if (player.School.AllPawns[i].Id == pawnId)
            {
                pawn = player.School.AllPawns[i];
            }
        }

        if (pawn == null)
        {
            throw new InvalidOperationException("There is no pawn with that ID");
        }

        for (int i = 0; i < player.MoveCards.Count; i++)
        {
            if (player.MoveCards[i].Name == moveCardName)
            {
                moveCard = player.MoveCards[i];
            }
        }

        if (moveCard == null)
        {
            throw new ApplicationException("There is no moveCard with that name for this player");
        }

        var list = PlayMat.GetValidMoves(pawn, moveCard, player.Direction);
        
        return list;
    }

    public IReadOnlyList<IMove> GetAllPossibleMovesFor(Guid playerId)
    {
        var list = new List<IMove>();
        IPlayer player = null;
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].Id == playerId)
            {
                player = Players[i];
            }
        }
        if (player == null)
        {
            throw new InvalidOperationException("There is no player with that ID");
        }

        for (int i = 0; i < player.School.AllPawns.Length; i++)
        {
            for (int j = 0; j < player.MoveCards.Count; j++)
            {
                var newList = PlayMat.GetValidMoves(player.School.AllPawns[i], player.MoveCards[j], player.Direction);
                for (int k = 0; k < newList.Count; k++)
                {
                    list.Add(newList[k]);
                }
            }
        }
        return list;
    }

    public void MovePawn(Guid playerId, Guid pawnId, string moveCardName, ICoordinate to)
    {
        throw new NotImplementedException();
    }

    public void SkipMovementAndExchangeCard(Guid playerId, string moveCardName)
    {
        throw new NotImplementedException();
    }

    public IPlayer GetNextOpponent(Guid playerId)
    {
        int k = 0;
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].Id == playerId) {
                k = i + 1; break;
            }
        }
        if(k > 2)
        {
            return _players.ElementAt(0);
        } else
        {
            return _players.ElementAt(k);
        }
    }
}