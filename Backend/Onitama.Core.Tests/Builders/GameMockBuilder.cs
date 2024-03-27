using Onitama.Core.GameAggregate.Contracts;

namespace Onitama.Core.Tests.Builders;

public class GameMockBuilder : MockBuilder<IGame>
{
    public GameMockBuilder()
    {
        Mock.SetupGet(t => t.Id).Returns(Guid.NewGuid());
    }
}