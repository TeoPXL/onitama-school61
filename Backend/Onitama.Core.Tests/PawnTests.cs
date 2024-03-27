using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Guts.Client.Core;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Tests.Extensions;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "Pawn",
    @"Onitama.Core\SchoolAggregate\Pawn.cs;")]
public class PawnTests
{
    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(Pawn).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_IPawn()
    {
        Assert.That(typeof(Pawn).IsAssignableTo(typeof(IPawn)), Is.True);
    }

    [MonitoredTest]
    public void IPawn_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(IPawn);
        type.AssertInterfaceProperty(nameof(IPawn.Id), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPawn.OwnerId), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPawn.Type), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IPawn.Position), shouldHaveGetter: true, shouldHaveSetter: true);
    }
}