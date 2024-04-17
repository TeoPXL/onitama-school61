namespace Onitama.Core.Util.Contracts;

public interface ICoordinateFactory
{
    ICoordinate Create(int row, int column);
}