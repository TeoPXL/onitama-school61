using System.Numerics;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Util;

/// <inheritdoc cref="ICoordinate"/>
internal class Coordinate : ICoordinate
{
    public int Row { get; }
    public int Column { get; }

    public Coordinate(int row, int column)
    {
        Row = row;
        Column = column;
    }

    //Do not change this method
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        return obj is ICoordinate other && Equals(other);
    }

    //Do not change this method
    protected bool Equals(ICoordinate other)
    {
        return Row == other.Row && Column == other.Column;
    }

    //Do not change this method
    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }

    //Do not change this method
    public override string ToString()
    {
        return $"({Row}, {Column})";
    }

    public bool IsOutOfBounds(int playMatSize)
    {
        if(Row > playMatSize || Column > playMatSize) return true;
        return false;
    }

    public ICoordinate GetNeighbor(Direction direction)
    {
        int newRow = Row + direction.YStep;
        int newColumn = Column + direction.YStep;
        return new Coordinate(newRow, newColumn);
    }

    public ICoordinate RotateTowards(Direction direction)
    {
        throw new NotImplementedException();
    }

    public int GetDistanceTo(ICoordinate other)
    {
        throw new NotImplementedException();
    }
}