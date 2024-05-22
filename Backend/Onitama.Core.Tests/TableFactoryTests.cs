using Guts.Client.Core;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.TableAggregate;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.Tests.Builders;
using Onitama.Core.Tests.Extensions;
using Onitama.Core.UserAggregate;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "TableFactory",
    @"Onitama.Core\TableAggregate\TableFactory.cs;
Onitama.Core\TableAggregate\TablePreferences.cs;")]
public class TableFactoryTests
{
    private TableFactory _tableFactory = null!;

    [SetUp]
    public void SetUp()
    {
        _tableFactory = new TableFactory();
    }

    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(TableFactory).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_ITableFactory()
    {
        Assert.That(typeof(TableFactory).IsAssignableTo(typeof(ITableFactory)), Is.True,
            "TableFactory should implement ITableFactory");
    }

    [MonitoredTest]
    public void ITableFactory_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(ITableFactory);
        type.AssertInterfaceMethod(nameof(ITableFactory.CreateNewForUser), typeof(ITable), typeof(User), typeof(TablePreferences));
    }

    [MonitoredTest]
    public void CreateNewForUser_ShouldCreateTableAndJoinUser()
    {
        Class_ShouldImplement_ITableFactory();

        // Arrange
        var user = new UserBuilder().Build();
        var preferences = new TablePreferences();

        // Act
        var table = _tableFactory.CreateNewForUser(user, preferences);

        // Assert
        Assert.That(table.Id, Is.Not.EqualTo(Guid.Empty), "A non-empty Guid must be used for the id");
        Assert.That(table.Preferences, Is.EqualTo(preferences), "The provided preferences must be assigned to the table");
        Assert.That(table.SeatedPlayers.Count, Is.EqualTo(1), "A player should be seated for the user");
        Assert.That(table.SeatedPlayers[0].Id, Is.EqualTo(user.Id), "A player should be seated for the user");
        Assert.That(table.OwnerPlayerId, Is.EqualTo(user.Id), "The player (user) that creates the table should be the owner");
        Assert.That(table.Id, Is.Not.EqualTo(table.OwnerPlayerId),
            "The id of the table must be unique. It can not be the same as the id of the owner");
    }
}