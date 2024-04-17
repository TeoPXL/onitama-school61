using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Util;

/// <summary>
/// Indicates a direction (e.g. North, South, ...)
/// </summary>
public class Direction
{
    public static Direction North = new Direction(0, 1); //Do not change
    public static Direction East = new Direction(1, 0); //Do not change
    public static Direction South = new Direction(0, -1); //Do not change
    public static Direction West = new Direction(-1, 0); //Do not change

    /// <summary>
    /// Array of the 4 main directions (North, South, East, West)
    /// </summary>
    public static Direction[] MainDirections => new[] { North, South, East, West }; //Do not change

    /// <summary>
    /// Horizontal direction.
    /// -1 = left
    /// 1 = right
    /// 0 = no horizontal direction 
    /// </summary>
    public int XStep { get; } //Do not change

    /// <summary>
    /// Vertical direction.
    /// 1 = up
    /// -1 = down
    /// 0 = no vertical direction 
    /// </summary>
    public int YStep { get; } //Do not change

    /// <summary>
    /// Angle in radians.
    /// </summary>
    public double AngleInRadians => Math.Atan2(XStep, YStep); //Do not change

    /// <summary>
    /// Get the direction perpendicular (loodrecht) to this direction
    /// </summary>
    public Direction PerpendicularDirection
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    //Do not change
    private Direction(int xStep, int yStep)
    {
        XStep = xStep;
        YStep = yStep;
    }

    //Implicit conversion operator to construct a direction from a string. Do not change
    public static implicit operator Direction(string direction)
    {
        return direction.ToLower() switch
        {
            "north" => North,
            "east" => East,
            "south" => South,
            "west" => West,
            "northeast" => North.CombineWith(East),
            "southeast" => South.CombineWith(East),
            "southwest" => South.CombineWith(West),
            "northwest" => North.CombineWith(West),
            _ => throw new System.ArgumentException("Invalid direction", nameof(direction)),
        };
    }

    /// <summary>
    /// Combines two directions into one (e.g. South + East = SouthEast)
    /// </summary>
    public Direction CombineWith(Direction other)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the start coordinate on a <see cref="IPlayMat"/> associated with this direction
    /// E.g. for the North direction, the start coordinate is in the middle of the bottom row (0, playMatSize).
    /// E.g. for the NorthEast direction, the start coordinate is at the left of the bottom row (0,0).
    /// E.g. for the SouthWest direction, the start coordinate is at the right of the top row (playMatSize,playMatSize).
    /// </summary>
    /// <param name="playMatSize">The size of the <see cref="IPlayMat"/>. Typically, 5.</param>
    public ICoordinate GetStartCoordinate(int playMatSize)
    {
        throw new NotImplementedException();
    }

    //Do not change
    public override string ToString()
    {
        if (YStep == 0)
        {
            return XStep == 1 ? "East" : "West";
        }

        string direction = YStep == 1 ? "North" : "South";
        if (XStep == -1) direction += "West";
        if (XStep == 1) direction += "East";
        return direction;

    }

    #region Equality overrides

    //Do not change
    public override bool Equals(object obj)
    {
        return Equals(obj as Direction);
    }

    //Do not change
    protected bool Equals(Direction other)
    {
        if (ReferenceEquals(other, null)) return false;
        return XStep == other.XStep && YStep == other.YStep;
    }

    //Do not change
    public static bool operator ==(Direction a, Direction b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        return a.Equals(b);
    }

    //Do not change
    public static bool operator !=(Direction a, Direction b)
    {
        return !(a == b);
    }

    //Do not change
    public override int GetHashCode()
    {
        return HashCode.Combine(XStep, YStep);
    }

    #endregion
}