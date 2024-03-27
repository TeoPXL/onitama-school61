using System.Drawing;
using Guts.Client.Core;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.TableAggregate;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.Tests.Builders;
using Onitama.Core.Tests.Extensions;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "Table",
    @"Onitama.Core\TableAggregate\Table.cs;
Onitama.Core\TableAggregate\TablePreferences.cs;")]
public class TableTests
{
    private Guid _id;
    private TablePreferences _defaultPreferences = null!;
    private ITable? _table;

    [SetUp]
    public void SetUp()
    {
        _id = Guid.NewGuid();
        _defaultPreferences = new TablePreferences();
        _table = new Table(_id, _defaultPreferences) as ITable;
    }

    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(Table).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_ITable()
    {
        Assert.That(typeof(Table).IsAssignableTo(typeof(ITable)), Is.True);
    }

    [MonitoredTest]
    public void ITable_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(ITable);

        type.AssertInterfaceProperty(nameof(ITable.Id), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(ITable.Preferences), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(ITable.OwnerPlayerId), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(ITable.SeatedPlayers), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(ITable.HasAvailableSeat), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(ITable.GameId), shouldHaveGetter: true, shouldHaveSetter: true);

        type.AssertInterfaceMethod(nameof(ITable.Join), typeof(void), typeof(User));
        type.AssertInterfaceMethod(nameof(ITable.Leave), typeof(void), typeof(Guid));
        type.AssertInterfaceMethod(nameof(ITable.FillWithArtificialPlayers), typeof(void), typeof(IGamePlayStrategy));
    }

    [MonitoredTest]
    public void Constructor_ShouldInitializeProperties()
    {
        Assert.That(_table, Is.Not.Null, "Table should implement ITable");
        Assert.That(_table!.Id, Is.EqualTo(_id), "Id is not set properly");
        Assert.That(_table.Preferences, Is.EqualTo(_defaultPreferences), "Preferences are not set properly");
        Assert.That(_table.OwnerPlayerId, Is.EqualTo(Guid.Empty), "There should be no owner when the table is empty");
        Assert.That(_table.HasAvailableSeat, Is.True, "A newly created table should have seats available");
    }

    [MonitoredTest]
    public void Join_FirstUserJoins_ShouldAddUserToSeatedPlayersAndMakeTheUserTheOwner()
    {
        Assert.That(_table, Is.Not.Null, "Table should implement ITable");

        // Arrange
        User user = new UserBuilder().Build();

        // Act
        _table!.Join(user);

        // Assert
        Assert.That(_table.SeatedPlayers.Count, Is.EqualTo(1), "There should be 1 seated player");
        IPlayer seatedPlayer = _table.SeatedPlayers[0];
        Assert.That(seatedPlayer.Id, Is.EqualTo(user.Id), "The seated player has an incorrect id");
        Assert.That(seatedPlayer.Name, Is.EqualTo(user.WarriorName), "The seated player has an incorrect name");
        Assert.That(seatedPlayer.Direction, Is.EqualTo(Direction.North), "The first player should get direction north");

        Assert.That(_table.OwnerPlayerId, Is.EqualTo(user.Id), "The owner of the table has an incorrect id");
    }

    [MonitoredTest]
    public void Join_SecondUserJoins_ShouldAddUserToSeatedPlayers()
    {
        Assert.That(_table, Is.Not.Null, "Table should implement ITable");

        // Arrange
        User user1 = new UserBuilder().Build();
        User user2 = new UserBuilder().Build();
        _table!.Join(user1);

        // Act
        _table.Join(user2);

        // Assert
        Assert.That(_table.SeatedPlayers.Count, Is.EqualTo(2), "There should be 2 seated players");
        IPlayer? secondPlayer = _table.SeatedPlayers.FirstOrDefault(p => p.Id == user2.Id);
        Assert.That(secondPlayer, Is.Not.Null, "The second player (with same id as the second user) should be seated");
        Assert.That(secondPlayer!.Name, Is.EqualTo(user2.WarriorName), "The second seated player has an incorrect name");
        Assert.That(secondPlayer.Direction, Is.EqualTo(Direction.South), "The second player should get direction south");
        Assert.That(_table.OwnerPlayerId, Is.EqualTo(user1.Id), "The owner of the table has an incorrect id");
        Assert.That(_table.HasAvailableSeat, Is.False, "The table should be full");
        Assert.That(_table.SeatedPlayers[0].Color, Is.Not.EqualTo(_table.SeatedPlayers[1].Color),
            "The colors of the players should be different");
    }

    [MonitoredTest]
    public void Join_UserJoinsTwice_ShouldThrowInvalidOperationException()
    {
        Assert.That(_table, Is.Not.Null, "Table should implement ITable");

        // Arrange
        User user = new UserBuilder().Build();
        _table!.Join(user);

        // Act + Assert
        Assert.That(() => _table.Join(user), Throws.InvalidOperationException);
        Assert.That(() => _table.Join(user),
           Throws.InvalidOperationException.With.Message.Contains("already seated").IgnoreCase,
           "The exception message should contain 'already seated'");
    }

    [MonitoredTest]
    public void Join_TableIsFull_ShouldThrowInvalidOperationException()
    {
        Assert.That(_table, Is.Not.Null, "Table should implement ITable");

        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var user3 = new UserBuilder().Build();
        _table!.Join(user1);
        _table.Join(user2);

        // Act + Assert
        Assert.That(() => _table.Join(user3), Throws.InvalidOperationException);
        Assert.That(() => _table.Join(user3),
            Throws.InvalidOperationException.With.Message.Contains("full").IgnoreCase,
            "The exception message should contain 'full'");
    }

    [MonitoredTest]
    public void Join_ShouldRandomlyPickAColor_RedBlueGreenYellowOrOrange()
    {
        Assert.That(_table, Is.Not.Null, "Table should implement ITable");

        Color[] possibleColors = new[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange };
        var colorCount = new Dictionary<Color, int>()
        {
            { Color.Red, 0 },
            { Color.Blue, 0 },
            { Color.Green, 0 },
            { Color.Yellow, 0 },
            { Color.Orange, 0 }
        };
        User user = new UserBuilder().Build();

        for (int i = 0; i < 100; i++)
        {
            _table = new Table(_id, _defaultPreferences) as ITable;
            _table!.Join(user);

            Assert.That(_table.SeatedPlayers.Count, Is.EqualTo(1), "There should be 1 seated player");
            IPlayer seatedPlayer = _table.SeatedPlayers[0];
            Assert.That(possibleColors, Contains.Item(seatedPlayer.Color), "The color of the seated player should be one of the possible colors");
            colorCount[seatedPlayer.Color]++;
        }

        Assert.That(colorCount.Values, Has.All.GreaterThan(0),
            "All possible colors should have been picked at least once after 100 join operations");
    }

    [MonitoredTest]
    public void Leave_ShouldRemoveUserFromSeatedPlayers()
    {
        Assert.That(_table, Is.Not.Null, "Table should implement ITable");

        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        _table!.Join(user1);
        _table.Join(user2);

        // Act
        _table.Leave(user1.Id);

        // Assert
        Assert.That(_table.SeatedPlayers.Count, Is.EqualTo(1), "There should be 1 seated player left");
        Assert.That(_table.SeatedPlayers[0].Id, Is.EqualTo(user2.Id), "The second user should still be seated");
        Assert.That(_table.OwnerPlayerId, Is.EqualTo(user2.Id), "The owner of the table should now be the user that didn't leave");
    }

    [MonitoredTest]
    public void Leave_UserThatIsNotSeatedTriesToLeave_ShouldThrowInvalidOperationException()
    {
        Assert.That(_table, Is.Not.Null, "Table should implement ITable");

        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        _table!.Join(user1);

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() => _table.Leave(user2.Id));
    }
}