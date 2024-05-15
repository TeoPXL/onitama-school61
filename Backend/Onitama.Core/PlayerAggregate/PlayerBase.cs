using System.Drawing;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;

namespace Onitama.Core.PlayerAggregate;

/// <inheritdoc cref="IPlayer"/>
internal class PlayerBase : IPlayer
{
    private ISchool _school;
    public Guid Id { get; }
    public string Name { get; }
    public Color Color { get; }
    public Direction Direction { get; }

    private int _elo;

    public virtual int Elo {
        get { return _elo; }
        set { _elo = value; }
    }

    private int _time = 180;
    public int Time {
        get { return _time; }
        set { _time = value; }
    }
    public ISchool School
    {
        get { return _school; }
    }
    public IList<IMoveCard> MoveCards { get; set; } 

    protected PlayerBase(Guid id, string name, Color color, Direction direction)
    {
        Id = id;
        Name = name;
        Color = color;
        Direction = direction;
        MoveCards = new List<IMoveCard>(); 
    }

    /// <summary>
    /// Creates a player that is a copy of an other player.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public PlayerBase(IPlayer otherPlayer)
    {
        throw new NotImplementedException("TODO: copy properties of other player");
    }

    public void SetSchool(ISchool school)
    {
        this._school = school;
    }
}