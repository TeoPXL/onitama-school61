using System.Drawing;
using Moq;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Tests.Builders;

public class MoveCardMockBuilder : MockBuilder<IMoveCard>
{
    public MoveCardMockBuilder()
    {
        Mock.SetupGet(card => card.Name).Returns(Guid.NewGuid().ToString());
        Mock.SetupGet(card => card.Grid).Returns(new MoveCardGridCellType[5, 5]);
        Mock.SetupGet(card => card.StampColor).Returns(Color.Red);
        Mock.Setup(card => card.GetPossibleTargetCoordinates(It.IsAny<ICoordinate>(), It.IsAny<Direction>(), It.IsAny<int>()))
            .Returns(new List<ICoordinate>());
    }

    public MoveCardMockBuilder WithColor(Color color)
    {
        Mock.SetupGet(card => card.StampColor).Returns(color);
        return this;
    }
}