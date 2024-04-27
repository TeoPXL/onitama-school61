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
        int x = this.Column;
        int y = this.Row;

        switch (direction)
        {
            case var d when d == Direction.North:
                int newXNorth = x;
                int newYNorth = y;
                return new Coordinate(newYNorth, newXNorth);

            case var d when d == Direction.West:
                int newXWest = -y;
                int newYWest = x;
                return new Coordinate(newYWest, newXWest);

            case var d when d == Direction.South:
                int newXSouth = -x;
                int newYSouth = -y;
                return new Coordinate(newYSouth, newXSouth);

            case var d when d == Direction.East:
                int newXEast = y;
                int newYEast = -x;
                return new Coordinate(newYEast, newXEast);

            default:
                throw new FormatException("Must use a valid direction.");
        }
    }
    private static double GetDistance(double x1, double y1, double x2, double y2)
    {
        return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
    }

    public int GetDistanceTo(ICoordinate other)
    {
        int x = this.Column;
        int y = this.Row;

        int xOther = other.Column;
        int yOther = other.Row;

        int distance = Convert.ToInt32(Math.Floor(GetDistance(x, y, xOther, yOther)));

        return distance;

    }
}