using Guts.Client.Core;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Tests.Extensions;
using Onitama.Core.Util;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "PlayMat",
    @"Onitama.Core\PlayMatAggregate\PlayMat.cs;")]
public class PlayMatTests
{
    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(PlayMat).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_IPlayMat()
    {
        Assert.That(typeof(PlayMat).IsAssignableTo(typeof(IPlayMat)), Is.True);
    }

    [MonitoredTest]
    public void IPlayMat_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(PlayMat);
        type.AssertInterfaceProperty(nameof(IPlayMat.Grid), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayMat.Size), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceMethod(nameof(IPlayMat.PositionSchoolOfPlayer), typeof(void), [typeof(IPlayer)]);
        type.AssertInterfaceMethod(nameof(IPlayMat.GetValidMoves), typeof(IReadOnlyList<IMove>), [typeof(IPawn), typeof(IMoveCard), typeof(Direction)]);
        type.AssertInterfaceMethod(nameof(IPlayMat.ExecuteMove), typeof(void), [typeof(IMove), typeof(IPawn)]);
    }
}