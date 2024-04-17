using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;

namespace Onitama.Core.Tests.Builders;

public class PlayMatMockBuilder : MockBuilder<IPlayMat>
{
    public PlayMatMockBuilder()
    {
        Mock.SetupGet(mat => mat.Size).Returns(5);
        Mock.SetupGet(mat => mat.Grid).Returns(new IPawn[5,5]);
    }
}