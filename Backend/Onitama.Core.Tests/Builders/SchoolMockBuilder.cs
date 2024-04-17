using Moq;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;

namespace Onitama.Core.Tests.Builders;

public class SchoolMockBuilder : MockBuilder<ISchool>
{
    private Guid _ownerId;
    private IPawn[] _allPaws = null!;

    public SchoolMockBuilder(Direction playDirection)
    {
        _ownerId = Guid.NewGuid();
        InitializePawnsForOwner();
        Mock.SetupGet(school => school.AllPawns).Returns(() => _allPaws);
        Mock.SetupGet(school => school.Master).Returns(() => _allPaws.First(p => p.Type == PawnType.Master));
        Mock.SetupGet(school => school.Students).Returns(() => _allPaws.Where(p => p.Type == PawnType.Student).ToArray());
        if (playDirection == Direction.North)
        {
            Mock.SetupGet(school => school.TempleArchPosition).Returns(new CoordinateMockBuilder(0,2).Object);
        }
        else
        {
            Mock.SetupGet(school => school.TempleArchPosition).Returns(new CoordinateMockBuilder(4,2).Object);
        }
        Mock.Setup(school => school.GetPawn(It.IsAny<Guid>())).Returns<Guid>(pawnId => _allPaws.FirstOrDefault(p => p.Id == pawnId));
    }

    public SchoolMockBuilder WithOwner(Guid ownerId)
    {
        _ownerId = ownerId;
        InitializePawnsForOwner();
        return this;
    }
    private void InitializePawnsForOwner()
    {
        _allPaws = [
            new PawnMockBuilder().WithOwner(_ownerId).WithType(PawnType.Student).Object,
            new PawnMockBuilder().WithOwner(_ownerId).WithType(PawnType.Student).Object,
            new PawnMockBuilder().WithOwner(_ownerId).WithType(PawnType.Student).Object,
            new PawnMockBuilder().WithOwner(_ownerId).WithType(PawnType.Student).Object,
            new PawnMockBuilder().WithOwner(_ownerId).WithType(PawnType.Master).Object
        ];
    }
}