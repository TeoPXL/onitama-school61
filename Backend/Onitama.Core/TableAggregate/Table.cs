using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;

namespace Onitama.Core.TableAggregate;

/// <inheritdoc cref="ITable"/>
internal class Table : ITable
{
    private Guid _id;
    private TablePreferences _preferences;
    private Guid _ownerPlayerId;
    private IList<IPlayer> _seatedPlayers = new List<IPlayer>();
    private bool _hasAvailableSeat;
    private Guid _gameId;
    private IList<Color> _availableColors = new List<Color>() {Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange };
    private IList<Direction> _availableDirections = new List<Direction>() { Direction.North, Direction.South, Direction.West, Direction.East };
    private static Random _random = new Random();


    private static readonly Color[] PossibleColors =
        new[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange };

    public Table(Guid id, TablePreferences preferences)
    {
        this._id = id;
        this._preferences = preferences;
        this._hasAvailableSeat = true;
    }
    public Guid get_id()
    {
        return this._id;
    }

    public Guid Id
    {
        get { return this._id; }
        set { this._id = value; }
    }

    public TablePreferences Preferences
    {
        get { return this._preferences; }
        set { this._preferences = value; }
    }

    public Guid OwnerPlayerId
    {
        get { return this._ownerPlayerId; }
        set { this._ownerPlayerId = value; }
    }

    public IList<IPlayer> SeatedPlayers
    {
        get { return this._seatedPlayers; }
        set { this._seatedPlayers = value; }
    }

    public bool HasAvailableSeat 
    {
        get { return this._hasAvailableSeat; }
        set { this._hasAvailableSeat = value; }
    }

    public Guid GameId
    {
        get { return this._gameId; }
        set { this.GameId = value; }
    }

    public void FillWithArtificialPlayers(IGamePlayStrategy gamePlayStrategy)
    {
        throw new NotImplementedException();
    }

    public void Join(User user)
    {
        if(_hasAvailableSeat == false)
        {
            throw new InvalidOperationException("There are no available seats! This table is full.");
        }
        foreach(var element in _seatedPlayers)
        {
            if (element.Id == user.Id)
            {
                throw new InvalidOperationException("This user is already seated at this table.");
            }
        }
        var number = _random.Next(0, _availableColors.Count);
        var color = _availableColors[number];

        var player = new HumanPlayer(user.Id, user.WarriorName, color, _availableDirections[0]);
        _availableDirections.RemoveAt(0);
        _availableColors.RemoveAt(number);
        if(_seatedPlayers.Count == 0)
        {
            this._ownerPlayerId = user.Id;
        }
        this._seatedPlayers.Add(player);
        if(_seatedPlayers.Count == _preferences.NumberOfPlayers)
        {
            _hasAvailableSeat = false;
        }
    }

    public void Leave(Guid userId)
    {
        for (int i = _seatedPlayers.Count - 1; i >= 0; i--)
        {
            var player = _seatedPlayers[i];
            if (player.Id == userId)
            {
                _seatedPlayers.RemoveAt(i);

                if (player.Id == _ownerPlayerId)
                {
                    _ownerPlayerId = _seatedPlayers[0].Id;
                    return;
                }
            }
        }
        throw new InvalidOperationException("This user is not at this table.");
    }
}