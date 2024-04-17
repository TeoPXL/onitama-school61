using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Tests.Builders;

public class CoordinateMockBuilder : MockBuilder<ICoordinate>
{
    public CoordinateMockBuilder(int row, int column)
    {
        Mock.SetupGet(c => c.Row).Returns(row);
        Mock.SetupGet(c => c.Column).Returns(column);
    }
}