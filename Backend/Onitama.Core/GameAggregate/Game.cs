using Microsoft.AspNetCore.Identity;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Threading;

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
    private string _gameType;
    private Timer _timer;
    private bool _isRunning;

    public UserEditor UserEditor { get; set; }

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

    public string Gametype
    {
        get { return this._gameType; }
        set { this._gameType = value; }
    }

    public string WinnerMethod { get; set; }

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
    /// <param name="gameType"></param>
    /// <param name="userEditor"></param>
    /// The type of the game (WayOfTheStone or WayOfTheWind)
    public Game(Guid id, IPlayMat playMat, IPlayer[] players, IMoveCard extraMoveCard, string gameType = "classic")
    {
        this._players = new IPlayer[players.Count()];

        for (int i = 0; i < _players.Length; i++)
        {
            _players[i] = players.ElementAt(i);
            _players[i].Time = 180;
        }
        this._id = id;  
        this._playMat = playMat;
        this._extraMoveCard = extraMoveCard;
        this._gameType = "WayOfTheStone";
        this.PlayerToPlayId = players[0].Id;
        this.PlayerToPlayId = players.FirstOrDefault(player => player.Color == _extraMoveCard.StampColor).Id;
        this._gameType = gameType;

        if (gameType == "blitz")
        {
            _timer = new Timer(UpdateTime, null, Timeout.Infinite, Timeout.Infinite);
            StartTimer();
        }
    }

    public void StartTimer()
    {
        if (!_isRunning)
        {
            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            _isRunning = true;
        }
    }

    public void StopTimer()
    {
        if (_isRunning)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);     
            _isRunning = false;

        }
    }

    public void UpdateTime(object state)
    {
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].Id == _playerToPlayId)
            {
                _players[i].Time--;

                if (_players[i].Time <= 0)
                {
                    _players[i].Time = 0;
                    for (int j = 0; j < _players.Length; j++)
                    {
                        if (_players[j].Id != _players[i].Id)
                        {
                            _winnerPlayerId = _players[j].Id;
                            WinnerMethod = "timer";
                        }
                    }
                }

            }
        }
    }

    public void Disposs()
    {
        _timer?.Dispose();
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
        //var list = new List<IMove>();

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
            if (player.School.AllPawns[i] != null && player.School.AllPawns[i].Id == pawnId)
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

        return PlayMat.GetValidMoves(pawn, moveCard, player.Direction);
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
        IPlayer player = null;
        IPawn pawn = null;
        IMoveCard moveCard = null;

        if (PlayerToPlayId != playerId)
        {
            throw new ApplicationException($"It is not this player's turn yet. {playerId} tried to play instead of {PlayerToPlayId}");
        }

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
            if (player.School.AllPawns[i] != null && player.School.AllPawns[i].Id == pawnId)
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
            throw new InvalidOperationException("There is no moveCard with that name for this player");
        }

        var move = new Move(moveCard, pawn, player.Direction, to);

        if(move == null)
        {
            throw new InvalidOperationException("Move is null");
        }

        var possibleMoves = PlayMat.GetValidMoves(pawn, moveCard, player.Direction);
        bool moveExists = false;
        if(possibleMoves != null)
        {
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                if (possibleMoves[i].Equals(move))
                {
                    moveExists = true;
                }
            }
            if (moveExists == false)
            {
                throw new InvalidOperationException("The move is invalid");
            }
        }
        
        

        IPawn capturedPawn;
        IList<ICoordinate> coordinates = new List<ICoordinate>();
        PlayMat.ExecuteMove(move, out capturedPawn);
        bool wayOfStream = false;

        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].School.TempleArchPosition.Row == move.To.Row && Players[i].School.TempleArchPosition.Column == move.To.Column)
            {
                wayOfStream = true;
            }
        }

        if (wayOfStream == true) //There might be an issue here
        {
            //Won by way of the wind, but for some reason we need to say "stream"... This inconsistency is very confusing
            WinnerPlayerId = playerId;
            WinnerMethod = "Way of the stream";
            calculateElo();

        } else if (capturedPawn != null)
        {
            if (capturedPawn.Type == PawnType.Master)
            {
                //Also kill the master!
                var capturedPlayerId = capturedPawn.OwnerId;
                for (int i = 0; i < _players.Length; i++)
                {
                    if (_players[i].Id == capturedPlayerId)
                    {
                        _players[i].School.RemovePawn(capturedPawn);
                        PlayMat.RemovePawn(capturedPawn);
                    }
                }
                WinnerPlayerId = playerId;
                WinnerMethod = "Way of the stone";
                calculateElo();
            }
            else if (capturedPawn.Type == PawnType.Student)
            {
                //Remove pawn
                var capturedPlayerId = capturedPawn.OwnerId;
                for (int i = 0; i < _players.Length; i++)
                {
                    if (_players[i].Id == capturedPlayerId)
                    {
                        _players[i].School.RemovePawn(capturedPawn);
                        PlayMat.RemovePawn(capturedPawn);
                    }
                }
            }



        }

        player.MoveCards.Remove(moveCard);
        player.MoveCards.Add(ExtraMoveCard);
        ExtraMoveCard = moveCard;
        PlayerToPlayId = this.GetNextOpponent(playerId).Id;
    }

    public void calculateElo()
    {
        int K = 64;
        int RA = 0;
        int RB = 0;
        int RC = 0;
        int RD = 0;
        double EA = 0;
        double EB = 0;
        double EC = 0;
        double ED = 0;
        RA = _players[0].Elo;
        RB = _players[1].Elo;
        if(_players.Length > 2)
        {
            RC= _players[0].Elo;
            RD = _players[0].Elo;
            // Calculate expected scores for each player (4 players)
            EA = 1 / (1 + Math.Pow(10, (RB - (RA + RC + RD) / 3.0) / 400.0));
            EB = 1 / (1 + Math.Pow(10, (RA - (RB + RC + RD) / 3.0) / 400.0));
            EC = 1 / (1 + Math.Pow(10, (RD - (RA + RB + RC) / 3.0) / 400.0));
            ED = 1 / (1 + Math.Pow(10, (RC - (RA + RB + RD) / 3.0) / 400.0));
        } else
        {
            // Calculate expected scores for each player (2 players)
            EA = 1 / (1 + Math.Pow(10, (RB - RA) / 400.0));
            EB = 1 / (1 + Math.Pow(10, (RA - RB) / 400.0));
        }
        int winningPlayer = 0;

        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].Id == WinnerPlayerId)
            {
                winningPlayer = i;
                break;
            }
        }
        

        // Initialize score variables
        double scoreA, scoreB, scoreC, scoreD;

        // Determine scores based on winning player
        switch (winningPlayer)
        {
            case 0:
                scoreA = 1;
                scoreB = 0;
                scoreC = 0;
                scoreD = 0;
                break;
            case 1:
                scoreA = 0;
                scoreB = 1;
                scoreC = 0;
                scoreD = 0;
                break;
            case 2:
                scoreA = 0;
                scoreB = 0;
                scoreC = 1;
                scoreD = 0;
                break;
            case 3:
                scoreA = 0;
                scoreB = 0;
                scoreC = 0;
                scoreD = 1;
                break;
            default:
                throw new ArgumentException("Invalid winning player");
        }
        // Update Elo ratings for each player
        if (_players.Length > 2)
        {
            double newRA = RA + K * (scoreA - EA);
            double newRB = RB + K * (scoreB - EB);
            double newRC = RC + K * (scoreC - EC);
            double newRD = RD + K * (scoreD - ED);
            _players[0].User.Elo = (Convert.ToInt32(newRA));
            _players[1].User.Elo = (Convert.ToInt32(newRB));
            _players[2].User.Elo = (Convert.ToInt32(newRC));
            _players[3].User.Elo = (Convert.ToInt32(newRD));
            //userEditor.UpdateUser(_players[0].User);
            //userEditor.UpdateUser(_players[1].User);
            //userEditor.UpdateUser(_players[2].User);
            //userEditor.UpdateUser(_players[3].User);
        } else
        {
            double newRA = RA + K * (scoreA - EA);
            double newRB = RB + K * (scoreB - EB);
            _players[0].User.Elo = (Convert.ToInt32(newRA));
            _players[1].User.Elo = (Convert.ToInt32(newRB));
            //userEditor.UpdateUser(_players[0].User);
            //userEditor.UpdateUser(_players[1].User);
        }
            
    }
    public async Task UpdateUsersAsync(UserManager<User> userManager)
    {
        foreach (var player in _players)
        {
            var result = await userManager.UpdateAsync(player.User);
            if (!result.Succeeded)
            {
                // Handle the case where the update operation failed
                throw new Exception("Could not update the database");
            }
        }
    }
    public void SkipMovementAndExchangeCard(Guid playerId, string moveCardName)
    {
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

        if (PlayerToPlayId != playerId)
        {
            throw new ApplicationException($"It is not this player's turn yet. {playerId} tried to play instead of {PlayerToPlayId}");
        }

        if (GetAllPossibleMovesFor(playerId).Count != 0)
        {
            throw new ApplicationException("The player can still do a valid move");
        }
        ExtraMoveCard = player.MoveCards[0];
        PlayerToPlayId = this.GetNextOpponent(playerId).Id;
    }

    public IPlayer GetNextOpponent(Guid playerId)
    {
        // Find the index of the player with the given ID
        int index = Array.FindIndex(_players, p => p.Id == playerId);

        if (index == -1)
        {
            // Player not found, handle the error accordingly
            return null; // or throw an exception
        }

        // Calculate the index of the next player, wrapping around if necessary
        int nextIndex = (index + 1) % _players.Length;

        return _players[nextIndex];
    }
}