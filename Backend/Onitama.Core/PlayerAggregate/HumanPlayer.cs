using System.Drawing;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;

namespace Onitama.Core.PlayerAggregate;

/// <inheritdoc cref="PlayerBase"/>
internal class HumanPlayer : PlayerBase
{
    private Guid _id;
    private string _name;
    private Color _color;
    private Direction _direction;
    private ISchool _school;
    private IList<IMoveCard> _moveCards;
    private int _elo;
    private User _user;
    private int _time = 180;

    public int Time
    {
        get { return _time; }
        set { _time = value; }
    }

    public HumanPlayer(Guid userId, string name, Color color, Direction direction, int elo): base(userId, name, color, direction)
    {
        _id = userId;
        _name = name;
        _color = color;
        _direction = direction;
        _elo = elo;
        _time = 180;
        this.Time = 180;
        this.Elo = elo;
    }

    public HumanPlayer(Guid userId, string name, Color color, Direction direction, int elo, User user) : base(userId, name, color, direction)
    {
        _id = userId;
        _name = name;
        _color = color;
        _direction = direction;
        _elo = elo;
        _user = user;
        _time = 180;
        this.Time = 180;
        this.Elo = elo;
    }

    public Guid Id
    { 
        get { return _id; } 
        set { _id = value; }
    }

    public string Name
    {
        get { return _name; }
        set { this._name = value; }
    }

    public Color Color
    {
        get { return _color; }
        set { this._color = value; }
    }

    public Direction Direction
    {
        get { return _direction; }
        set { this._direction = value; }
    }

    public ISchool School
    {
        get { return _school; }
    }

    public IList<IMoveCard> MoveCards
    {
        get { return _moveCards; }
        set { this._moveCards = value; }
    }

    public override int Elo
    {
        get { return _elo; }
        set { _elo = value; }
    }

    public User User
    {
        get { return _user; }
        set { this._user = value; }
    }
}