using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.Util;

namespace Onitama.Core.Tests.Builders;

public class MoveMockBuilder : MockBuilder<IMove>
{
    public MoveMockBuilder()
    {
        Mock.SetupGet(m => m.Pawn).Returns(new PawnMockBuilder().Object);
        Mock.SetupGet(m => m.Card).Returns(new MoveCardMockBuilder().Object);
        Mock.SetupGet(m => m.To).Returns(new CoordinateMockBuilder(Random.Shared.Next(0,5), Random.Shared.Next(0, 5)).Object);
    }
}