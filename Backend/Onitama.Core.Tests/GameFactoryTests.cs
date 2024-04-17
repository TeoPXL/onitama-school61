using System.Drawing;
using Guts.Client.Core;
using Moq;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.Tests.Builders;
using Onitama.Core.Tests.Extensions;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "GameFactory",
    @"Onitama.Core\GameAggregate\GameFactory.cs;")]
public class GameFactoryTests
{
    private GameFactory _gameFactory = null!;
    private Mock<IMoveCardRepository> _moveCardRepositoryMock = null!;
    private ITable _table = null!;
    private Color[] _colors = null!;

    [SetUp]
    public void SetUp()
    {
        User userA = new UserBuilder().WithWarriorName("WarriorA").Build();
        User userB = new UserBuilder().WithWarriorName("WarriorB").Build();
        _table = new TableMockBuilder().WithSeatedUsers([userA, userB]).Object;
        _colors = _table.SeatedPlayers.Select(p => p.Color).ToArray();

        _moveCardRepositoryMock = new Mock<IMoveCardRepository>();


        var allMoveCards = new List<IMoveCard>();
        for (int i = 0; i < 15; i++)
        {
            allMoveCards.Add(new MoveCardMockBuilder().WithColor(Random.Shared.NextItem(_colors)).Object);
        }

        _moveCardRepositoryMock.Setup(repo => repo.LoadSet(It.IsAny<MoveCardSet>(), It.IsAny<Color[]>()))
            .Returns(allMoveCards.ToArray);

        _gameFactory = new GameFactory(_moveCardRepositoryMock.Object);
    }

    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(GameFactory).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_IGameFactory()
    {
        Assert.That(typeof(GameFactory).IsAssignableTo(typeof(IGameFactory)), Is.True);
    }

    [MonitoredTest]
    public void IGameFactory_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(IGameFactory);
        type.AssertInterfaceMethod(nameof(IGameFactory.CreateNewForTable), typeof(IGame), typeof(ITable));
    }

    [MonitoredTest]
    public void CreateNewForTable_ShouldInitializeBasicProperties()
    {
        // Act
        IGame? game = _gameFactory.CreateNewForTable(_table);

        // Assert
        Assert.That(game.Id, Is.Not.EqualTo(Guid.Empty), "A non-empty Guid must be used for the id");
        foreach (IPlayer player in _table.SeatedPlayers)
        {
            IPlayer? matchingPlayer = game.Players.FirstOrDefault(p => p.Id == player.Id);
            Assert.That(matchingPlayer, Is.Not.Null, "Each player in the game should be one of the players seated at the table");
        }
        Assert.That(game.PlayMat, Is.Not.Null, "A play mat should be created for the game");
        Assert.That(game.PlayMat!.Size, Is.EqualTo(_table.Preferences.PlayerMatSize),
            $"The size of the play mat should be {_table.Preferences.PlayerMatSize}");
    }

    [MonitoredTest]
    public void CreateNewForTable_ShouldPositionTheSchoolOfThePlayersOnTheMat()
    {
        // Act
        IGame? game = _gameFactory.CreateNewForTable(_table);

        // Assert
        foreach (IPlayer player in game.Players)
        {
            int expectedRow = player.Direction == Direction.North ? 0 : game.PlayMat.Size - 1;

            Assert.That(player.School, Is.Not.Null, "The school of the players must have a value");
            Assert.That(player.School.AllPawns.Length, Is.EqualTo(5), "Each player school should have 5 pawns");

            Assert.That(player.School.AllPawns.All(pawn => pawn.Position != null),
                "All of the pawns of the players must have a position");

            Assert.That(player.School.AllPawns.All(pawn => pawn.Position.Row == expectedRow),
                $"All of the pawns of the player playing in the '{player.Direction}' direction are expected to be in row {expectedRow}");

            foreach (IPawn pawn in player.School.AllPawns)
            {
                Assert.That(game.PlayMat.Grid[pawn.Position.Row, pawn.Position.Column], Is.SameAs(pawn),
                    $"A pawn of the school of a player cannot be found in the grid of the play mat");
            }
        }
    }

    [MonitoredTest]
    public void CreateNewForTable_ShouldRandomlyChoose5MoveCardAndDistributeThem()
    {
        IMoveCard?[] previousChosenMoveCards = new IMoveCard?[5];

        int numberOfDifferentMoveCardChoices = 0;
        int numberOfGameCreations = 10;

        for (int i = 0; i < numberOfGameCreations; i++)
        {
            foreach (IPlayer player in _table.SeatedPlayers)
            {
                player.MoveCards?.Clear();
            }

            _moveCardRepositoryMock.Invocations.Clear();

           IGame? game = _gameFactory.CreateNewForTable(_table);

            _moveCardRepositoryMock.Verify(repo => repo.LoadSet(MoveCardSet.Original, _colors), Times.Once,
                "The 'LoadSet' method of the constructor-injected repository is not called correctly");

            Assert.That(game.Players.All(p => p.MoveCards is { Count: 2 }), "Each player should have 2 move cards");
            Assert.That(game.ExtraMoveCard, Is.Not.Null, "An extra move card should be chosen and assigned to the game");

            IMoveCard[] chosenMoveCards = game.Players.SelectMany(p => p.MoveCards).Concat(new[] { game.ExtraMoveCard }).ToArray();

            if (!chosenMoveCards.All(chosenCard =>
                    previousChosenMoveCards.Any(
                        previousCard => previousCard != null && previousCard.Name == chosenCard.Name)))
            {
                numberOfDifferentMoveCardChoices++;
            }
            previousChosenMoveCards = chosenMoveCards;
        }

        Assert.That(numberOfDifferentMoveCardChoices, Is.GreaterThanOrEqualTo(8),
                       "The chosen move cards should be different in at least 8 out of 10 game creations");
    }
}