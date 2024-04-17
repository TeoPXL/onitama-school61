using System.Drawing;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;

namespace Onitama.Core.PlayerAggregate;

/// <inheritdoc cref="IPlayer"/>
internal class HumanPlayer : IPlayer
{
    private Guid _id;
    private string _name;
    private Color _color;
    private Direction _direction;
    private ISchool _school;
    private IList<IMoveCard> _moveCards;

    public HumanPlayer(Guid userId, string name, Color color, Direction direction)
    {
        _id = userId;
        _name = name;
        _color = color;
        _direction = direction;
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
        set { this.Direction = value; }
    }

    public ISchool School
    {
        get { return _school; }
        set { this._school = value; }
    }

    public IList<IMoveCard> MoveCards
    {
        get { return _moveCards; }
        set { this._moveCards = value; }
    }
}