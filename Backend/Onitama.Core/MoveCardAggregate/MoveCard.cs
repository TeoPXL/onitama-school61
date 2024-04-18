using System.Drawing;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.MoveCardAggregate;

/// <inheritdoc cref="IMoveCard"/>
internal class MoveCard : IMoveCard
{
    private Color _stampColor;
    private MoveCardGridCellType[,] _grid;
    public string Name { get; }

    public MoveCardGridCellType[,] Grid
    {
        get { return _grid; }
        set { this._grid = value; }
    }

    public Color StampColor
    {
        get { return _stampColor; }
        set { this._stampColor = value; }
    }

    public MoveCard(string name, MoveCardGridCellType[,] grid, Color stampColor)
    {
        this._grid = grid;
        this._stampColor = stampColor;
        this.Name = name;
    }

    //Do not change this method, it makes sure that two MoveCard instances are equal if their names are equal
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        return obj is IMoveCard other && Equals(other);
    }

    //Do not change this method
    protected bool Equals(IMoveCard other)
    {
        return Name == other.Name;
    }

    //Do not change this method
    public override int GetHashCode()
    {
        return (Name != null ? Name.GetHashCode() : 0);
    }

    public IReadOnlyList<ICoordinate> GetPossibleTargetCoordinates(ICoordinate startCoordinate, Direction playDirection, int matSize)
    {
        throw new NotImplementedException();
    }
}