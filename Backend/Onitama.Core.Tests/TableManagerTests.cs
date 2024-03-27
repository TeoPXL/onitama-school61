using NUnit.Framework;
using Onitama.Core.TableAggregate;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Moq;
using System;
using Guts.Client.Core;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.Tests.Builders;
using Onitama.Core.Tests.Extensions;
using Onitama.Core.Util;

namespace Onitama.Core.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Onitama", "TableManager",
        @"Onitama.Core\TableAggregate\TableManager.cs;")]
    public class TableManagerTests
    {
        private Mock<ITableRepository> _tableRepositoryMock = null!;
        private Mock<ITableFactory> _tableFactoryMock = null!;
        private Mock<IGameRepository> _gameRepositoryMock = null!;
        private Mock<IGameFactory> _gameFactoryMock = null!;
        private Mock<IGamePlayStrategy> _gamePlayStrategyMock = null!;
        private TableManager _tableManager = null!;

        [SetUp]
        public void Setup()
        {
            _tableRepositoryMock = new Mock<ITableRepository>();
            _tableFactoryMock = new Mock<ITableFactory>();
            _gameRepositoryMock = new Mock<IGameRepository>();
            _gameFactoryMock = new Mock<IGameFactory>();
            _gamePlayStrategyMock = new Mock<IGamePlayStrategy>();

            _tableManager = new TableManager(
                _tableRepositoryMock.Object,
                _tableFactoryMock.Object,
                _gameRepositoryMock.Object,
                _gameFactoryMock.Object,
                _gamePlayStrategyMock.Object);
        }

        [MonitoredTest]
        public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
        {
            Assert.That(typeof(TableManager).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
        }

        [MonitoredTest]
        public void Class_ShouldImplement_ITableManager()
        {
            Assert.That(typeof(TableManager).IsAssignableTo(typeof(ITableManager)), Is.True);
        }

        [MonitoredTest]
        public void ITableManager_Interface_ShouldHaveCorrectMembers()
        {
            var type = typeof(ITableManager);

            type.AssertInterfaceMethod(nameof(ITableManager.AddNewTableForUser), typeof(ITable), typeof(User), typeof(TablePreferences));
            type.AssertInterfaceMethod(nameof(ITableManager.JoinTable), typeof(void), typeof(Guid), typeof(User));
            type.AssertInterfaceMethod(nameof(ITableManager.LeaveTable), typeof(void), typeof(Guid), typeof(User));
            type.AssertInterfaceMethod(nameof(ITableManager.FillWithArtificialPlayers), typeof(void), typeof(Guid), typeof(User));
            type.AssertInterfaceMethod(nameof(ITableManager.StartGameForTable), typeof(IGame), typeof(Guid), typeof(User));
        }

        [MonitoredTest]
        public void AddNewTableForUser_ShouldCreateAndAddNewTable()
        {
            // Arrange
            User user = new UserBuilder().Build();
            TablePreferences preferences = new TablePreferencesBuilder().Build();
            ITable table = new TableMockBuilder().Object;

            _tableFactoryMock.Setup(f => f.CreateNewForUser(It.IsAny<User>(), It.IsAny<TablePreferences>())).Returns(table);

            // Act
            var result = _tableManager.AddNewTableForUser(user, preferences);

            // Assert
            _tableFactoryMock.Verify(f => f.CreateNewForUser(user, preferences), Times.Once, "The factory should be used (correctly) to create a table");
            _tableRepositoryMock.Verify(r => r.Add(table), Times.Once, "The created table should be added to the table repository");
            Assert.That(result, Is.SameAs(table), "The created table should be returned");
        }

        [MonitoredTest]
        public void JoinTable_ShouldAddUserToTable()
        {
            // Arrange
            User user = new UserBuilder().Build();
            var tableMockBuilder = new TableMockBuilder();
            Mock<ITable> tableMock = tableMockBuilder.Mock;
            ITable table = tableMockBuilder.Object;

            _tableRepositoryMock.Setup(r => r.Get(It.Is<Guid>(id => id != table.Id))).Throws<DataNotFoundException>();
            _tableRepositoryMock.Setup(r => r.Get(table.Id)).Returns(table);

            // Act
            _tableManager.JoinTable(table.Id, user);

            // Assert
            _tableRepositoryMock.Verify(repository => repository.Get(table.Id), Times.Once, "Table is not retrieved correctly");
            tableMock.Verify(t => t.Join(user), Times.Once, "User is not joined to the table correctly");
        }

        [MonitoredTest]
        public void LeaveTable_FirstOfTwoPlayersLeaves_ShouldRemovePlayerAssociatedWithUserFromTable()
        {
            // Arrange
            User user1 = new UserBuilder().Build();
            User user2 = new UserBuilder().Build();
            var tableMockBuilder = new TableMockBuilder().WithSeatedUsers([user1, user2]);
            Mock<ITable> tableMock = tableMockBuilder.Mock;
            ITable table = tableMockBuilder.Object;

            _tableRepositoryMock.Setup(r => r.Get(It.Is<Guid>(id => id != table.Id))).Throws<DataNotFoundException>();
            _tableRepositoryMock.Setup(r => r.Get(table.Id)).Returns(table);

            // Act
            _tableManager.LeaveTable(table.Id, user2);

            // Assert
            _tableRepositoryMock.Verify(repository => repository.Get(table.Id), Times.Once, "Table is not retrieved correctly");
            tableMock.Verify(t => t.Leave(user2.Id), Times.Once, "The 'Leave' method of the table is not called correctly");
            _tableRepositoryMock.Verify(repository => repository.Remove(It.IsAny<Guid>()), Times.Never,
                "The table should not be removed from the repository when there is still a player present");
        }

        [MonitoredTest]
        public void LeaveTable_LastPlayersLeaves_ShouldRemoveTableFromRepository()
        {
            // Arrange
            User user = new UserBuilder().Build();
            var tableMockBuilder = new TableMockBuilder().WithSeatedUsers([user]);
            Mock<ITable> tableMock = tableMockBuilder.Mock;
            ITable table = tableMockBuilder.Object;

            _tableRepositoryMock.Setup(r => r.Get(It.Is<Guid>(id => id != table.Id))).Throws<DataNotFoundException>();
            _tableRepositoryMock.Setup(r => r.Get(table.Id)).Returns(table);

            // Act
            _tableManager.LeaveTable(table.Id, user);

            // Assert
            _tableRepositoryMock.Verify(repository => repository.Get(table.Id), Times.Once, "Table is not retrieved correctly");
            tableMock.Verify(t => t.Leave(user.Id), Times.Once, "The 'Leave' method of the table is not called correctly");
            _tableRepositoryMock.Verify(repository => repository.Remove(table.Id), Times.Once,
                "The table is not removed correctly from the repository");
        }

        [MonitoredTest]
        public void StartGameForTable_ShouldUseFactoryToCreateAGameAndAddItToTheRepository()
        {
            // Arrange
            User user1 = new UserBuilder().Build();
            User user2 = new UserBuilder().Build();
            var tableMockBuilder = new TableMockBuilder().WithSeatedUsers([user1, user2]);
            Mock<ITable> tableMock = tableMockBuilder.Mock;
            ITable table = tableMockBuilder.Object;
            
            _tableRepositoryMock.Setup(r => r.Get(It.Is<Guid>(id => id != table.Id))).Throws<DataNotFoundException>();
            _tableRepositoryMock.Setup(r => r.Get(table.Id)).Returns(table);

            IGame createdGame = new GameMockBuilder().Object;
            _gameFactoryMock.Setup(f => f.CreateNewForTable(table)).Returns(createdGame);

            // Act
            IGame returnedGame = _tableManager.StartGameForTable(table.Id, user1);

            // Assert
            _tableRepositoryMock.Verify(repository => repository.Get(table.Id), Times.Once, "Table is not retrieved correctly");
            _gameFactoryMock.Verify(f => f.CreateNewForTable(table), Times.Once, "The 'CreateNewForTable' method of the game factory is not called correctly");
            tableMock.VerifySet(t => t.GameId = returnedGame.Id, Times.Once, "The GameId of the table is not set correctly");
            _gameRepositoryMock.Verify(r => r.Add(createdGame), Times.Once, "The created game is not added to the game repository");
            Assert.That(returnedGame, Is.SameAs(createdGame), "The created game is not returned correctly");
        }

        [MonitoredTest]
        public void StartGameForTable_UserIsNotTheOwner_ShouldThrowInvalidOperationException()
        {
            // Arrange
            User owner = new UserBuilder().Build();
            User otherUser = new UserBuilder().Build();
            var tableMockBuilder = new TableMockBuilder().WithSeatedUsers([owner, otherUser]);
            Mock<ITable> tableMock = tableMockBuilder.Mock;
            ITable table = tableMockBuilder.Object;

            _tableRepositoryMock.Setup(r => r.Get(It.Is<Guid>(id => id != table.Id))).Throws<DataNotFoundException>();
            _tableRepositoryMock.Setup(r => r.Get(table.Id)).Returns(table);

            // Act + Assert
            Assert.That(() => _tableManager.StartGameForTable(table.Id, otherUser),
                Throws.InvalidOperationException.With.Message.Contains("owner").IgnoreCase,
                "The exception message should contain the word 'owner'");

            _tableRepositoryMock.Verify(repository => repository.Get(table.Id), Times.Once, "Table is not retrieved correctly");
            _gameRepositoryMock.Verify(r => r.Add(It.IsAny<IGame>()), Times.Never, "A game was added to the repository");
        }

        [MonitoredTest]
        public void StartGameForTable_TableIsNotFull_ShouldThrowInvalidOperationException()
        {
            // Arrange
            User owner = new UserBuilder().Build();
            var tableMockBuilder = new TableMockBuilder().WithSeatedUsers([owner]);
            Mock<ITable> tableMock = tableMockBuilder.Mock;
            ITable table = tableMockBuilder.Object;

            _tableRepositoryMock.Setup(r => r.Get(It.Is<Guid>(id => id != table.Id))).Throws<DataNotFoundException>();
            _tableRepositoryMock.Setup(r => r.Get(table.Id)).Returns(table);

            // Act + Assert
            Assert.That(() => _tableManager.StartGameForTable(table.Id, owner),
                Throws.InvalidOperationException.With.Message.Contains("not enough").IgnoreCase,
                "The exception message should contain the words 'not enough'");

            _tableRepositoryMock.Verify(repository => repository.Get(table.Id), Times.Once, "Table is not retrieved correctly");
            _gameRepositoryMock.Verify(r => r.Add(It.IsAny<IGame>()), Times.Never, "A game was added to the repository");
        }
    }
}
