using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Util;

internal class CoordinateFactory : ICoordinateFactory
{
    public ICoordinate Create(int row, int column)
    {
        return new Coordinate(row, column);
    }
}