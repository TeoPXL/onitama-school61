using Guts.Client.Core;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Tests.Extensions;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "School",
    @"Onitama.Core\SchoolAggregate\School.cs;")]
public class SchoolTests
{
    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(School).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_ISchool()
    {
        Assert.That(typeof(School).IsAssignableTo(typeof(ISchool)), Is.True);
    }

    [MonitoredTest]
    public void ISchool_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(ISchool);
        type.AssertInterfaceProperty(nameof(ISchool.Master), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(ISchool.Students), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(ISchool.AllPawns), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(ISchool.TempleArchPosition), shouldHaveGetter: true, shouldHaveSetter: true);
        type.AssertInterfaceMethod(nameof(ISchool.GetPawn), typeof(IPawn), [typeof(Guid)]);
    }
}