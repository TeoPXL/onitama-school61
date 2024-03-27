using Onitama.Core.SchoolAggregate.Contracts;

namespace Onitama.Core.Tests.Builders;

public class PawnMockBuilder : MockBuilder<IPawn>
{
    public PawnMockBuilder()
    {
        Mock.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        Mock.SetupGet(p => p.OwnerId).Returns(Guid.NewGuid());
        Mock.SetupProperty(p => p.Position, null);
        Mock.SetupGet(p => p.Type).Returns(PawnType.Student);
    }

    public PawnMockBuilder WithOwner(Guid ownerId)
    {
        Mock.SetupGet(p => p.OwnerId).Returns(ownerId);
        return this;
    }

    public PawnMockBuilder WithType(PawnType type)
    {
        Mock.SetupGet(p => p.Type).Returns(type);
        return this;
    }
}