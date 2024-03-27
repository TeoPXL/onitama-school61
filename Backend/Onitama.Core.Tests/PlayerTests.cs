using Guts.Client.Core;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate;
using Onitama.Core.Tests.Extensions;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "Player",
    @"Onitama.Core\PlayerAggregate\PlayerBase.cs;
Onitama.Core\PlayerAggregate\HumanPlayer.cs;")]
public class PlayerTests
{
    [MonitoredTest]
    public void HumanPlayer_Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(HumanPlayer).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void HumanPlayer_Class_ShouldInheritFromPlayerBase()
    {
        Assert.That(typeof(HumanPlayer).IsAssignableTo(typeof(PlayerBase)), Is.True);
    }

    [MonitoredTest]
    public void PlayerBase_Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(PlayerBase).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void PlayerBase_Class_ShouldImplement_IPlayer()
    {
        Assert.That(typeof(PlayerBase).IsAssignableTo(typeof(IPlayer)), Is.True);
    }

    [MonitoredTest]
    public void IPlayer_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(IPlayer);
        type.AssertInterfaceProperty(nameof(IPlayer.Id), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayer.Name), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayer.Color), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayer.Direction), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayer.School), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPlayer.MoveCards), shouldHaveGetter: true, shouldHaveSetter: false);
    }
}