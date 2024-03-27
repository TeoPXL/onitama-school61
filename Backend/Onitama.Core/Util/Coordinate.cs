using System.Numerics;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Util;

/// <inheritdoc cref="ICoordinate"/>
internal class Coordinate
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
}