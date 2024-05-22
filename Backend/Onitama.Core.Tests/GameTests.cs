using System.Drawing;
using Guts.Client.Core;
using Moq;
using NUnit.Framework.Constraints;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Tests.Builders;
using Onitama.Core.Tests.Extensions;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "Game",
    @"Onitama.Core\GameAggregate\Game.cs;")]
public class GameTests
{
    private Mock<IMoveCard> _extraMoveCardMock = null!;
    private Mock<IPlayMat> _playMatMock = null!;
    private Mock<IPlayer> _playerNorthMock = null!;
    private Mock<IPlayer> _playerSouthMock = null!;
    private IGame _game = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _extraMoveCardMock = new MoveCardMockBuilder().WithColor(Color.Blue).Mock;
        _playMatMock = new PlayMatMockBuilder().Mock;
        _playerNorthMock = new PlayerMockBuilder()
            .WithColor(Color.Blue)
            .WithMoveCards()
            .WithDirection(Direction.North).Mock;
        _playerSouthMock = new PlayerMockBuilder()
            .WithColor(Color.Red)
            .WithMoveCards()
            .WithDirection(Direction.South).Mock;
        IPlayer[] players = [_playerNorthMock.Object, _playerSouthMock.Object];
        _game = (new Game(Guid.NewGuid(), _playMatMock.Object, players, _extraMoveCardMock.Object) as IGame)!;
    }

    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(Game).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_IGame()
    {
        Assert.That(typeof(Game).IsAssignableTo(typeof(IGame)), Is.True, "Game should implement IGame");
    }

    [MonitoredTest]
    public void IGame_Interface_ShouldHaveCorrectMembers()
    {
        var type = typeof(IGame);
        type.AssertInterfaceProperty(nameof(IGame.Id), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IGame.PlayMat), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IGame.ExtraMoveCard), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IGame.Players), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IGame.PlayerToPlayId), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IGame.WinnerPlayerId), shouldHaveGetter: true, shouldHaveSetter: false);
        type.AssertInterfaceProperty(nameof(IGame.WinnerMethod), shouldHaveGetter: true, shouldHaveSetter: false);

        type.AssertInterfaceMethod(nameof(IGame.GetPossibleMovesForPawn), typeof(IReadOnlyList<IMove>), [typeof(Guid), typeof(Guid), typeof(string)]);
        type.AssertInterfaceMethod(nameof(IGame.GetAllPossibleMovesFor), typeof(IReadOnlyList<IMove>), [typeof(Guid)]);
        type.AssertInterfaceMethod(nameof(IGame.MovePawn), typeof(void), [typeof(Guid), typeof(Guid), typeof(string), typeof(ICoordinate)]);
        type.AssertInterfaceMethod(nameof(IGame.SkipMovementAndExchangeCard), typeof(void), [typeof(Guid), typeof(string)]);
        type.AssertInterfaceMethod(nameof(IGame.GetNextOpponent), typeof(IPlayer), [typeof(Guid)]);
    }

    [MonitoredTest]
    public void GetPossibleMovesForPawn_InvalidPlayerId_ShouldThrowInvalidOperationException()
    {
        Class_ShouldImplement_IGame();

        //Arrange
        Guid invalidPlayerId = Guid.NewGuid();
        IPawn pawn = Random.Shared.NextItem(_playerNorthMock.Object.School.AllPawns);
        IMoveCard moveCard = Random.Shared.NextItem(_playerNorthMock.Object.MoveCards);

        //Act + Assert
        Assert.That(() => _game.GetPossibleMovesForPawn(invalidPlayerId, pawn.Id, moveCard.Name),
                       Throws.InvalidOperationException.With.Message.Contains("player").IgnoreCase);
    }

    [MonitoredTest]
    public void GetPossibleMovesForPawn_PlayerIsNotInPossessionOfTheCard_ShouldThrowApplicationException()
    {
        Class_ShouldImplement_IGame();

        //Arrange
        string invalidCardName = _playerSouthMock.Object.MoveCards.First().Name;
        IPawn pawn = Random.Shared.NextItem(_playerNorthMock.Object.School.AllPawns);

        //Act + Assert
        Assert.That(() => _game.GetPossibleMovesForPawn(_playerNorthMock.Object.Id, pawn.Id, invalidCardName),
            Throws.InstanceOf<ApplicationException>().With.Message.Contains("card").IgnoreCase);
    }

    [MonitoredTest]
    public void GetPossibleMovesForPawn_PlayerIsInPossessionOfTheCard_ShouldUsePlayMatToGetTheValidMoves()
    {
        Class_ShouldImplement_IGame();
        //Arrange
        IMoveCard card = _playerNorthMock.Object.MoveCards.ElementAt(1);
        IPawn pawn = Random.Shared.NextItem(_playerNorthMock.Object.School.AllPawns);

        IReadOnlyList<IMove> validMoves = [new MoveMockBuilder().Object];
        _playMatMock.Setup(p => p.GetValidMoves(It.IsAny<IPawn>(), It.IsAny<IMoveCard>(), It.IsAny<Direction>())).Returns(validMoves);

        //Act
        IReadOnlyList<IMove> returnedMoves = _game.GetPossibleMovesForPawn(_playerNorthMock.Object.Id, pawn.Id, card.Name);

        //Assert
        _playMatMock.Verify(p => p.GetValidMoves(pawn, card, _playerNorthMock.Object.Direction), Times.Once,
            "The play mat is not used correctly to retrieve the valid moves.");
        Assert.That(returnedMoves, Is.SameAs(validMoves),
            "The moves returned by the play mat should be the same list that is returned");
    }

    [MonitoredTest]
    public void GetAllPossibleMovesFor_ShouldUsePlayMatToGetTheValidMoves()
    {
        Class_ShouldImplement_IGame();

        //Arrange
        int pawnCount = _playerNorthMock.Object.School.AllPawns.Length;
        IReadOnlyList<IMove> validMoves = [new MoveMockBuilder().Object];
        _playMatMock.Setup(p => p.GetValidMoves(It.IsAny<IPawn>(), It.IsAny<IMoveCard>(), It.IsAny<Direction>())).Returns(validMoves);

        //Act
        IReadOnlyList<IMove> returnedMoves = _game.GetAllPossibleMovesFor(_playerNorthMock.Object.Id);

        //Assert
        _playMatMock.Verify(p => p.GetValidMoves(It.IsIn(_playerNorthMock.Object.School.AllPawns), It.IsIn<IMoveCard>(_playerNorthMock.Object.MoveCards), _playerNorthMock.Object.Direction), Times.Exactly(pawnCount * 2),
            $"The play mat should be uses {pawnCount * 2} times to retrieve the valid moves.");
        Assert.That(returnedMoves, Has.Count.EqualTo(pawnCount * 2),
            "All the moves returned by the play mat should be combined in the list that is returned");
    }

    [MonitoredTest]
    public void MovePawn_ItIsNotThePlayersTurn_ShouldThrowApplicationException()
    {
        Class_ShouldImplement_IGame();

        //Arrange
        Guid playerNotAtTurnId = _game.PlayerToPlayId == _playerNorthMock.Object.Id ? _playerSouthMock.Object.Id : _playerNorthMock.Object.Id;
        IMoveCard card = Random.Shared.NextItem(_playerNorthMock.Object.MoveCards);
        IPawn pawn = Random.Shared.NextItem(_playerNorthMock.Object.School.AllPawns);
        ICoordinate to = (new Coordinate(2, 2) as ICoordinate)!;

        //Act + Assert
        Assert.That(() => _game.MovePawn(playerNotAtTurnId, pawn.Id, card.Name, to),
            Throws.InstanceOf<ApplicationException>().With.Message.Contains("turn").IgnoreCase);
    }

    [MonitoredTest]
    public void MovePawn_WinningMoveByWayOfTheWind_ShouldSetWinner()
    {
        Class_ShouldImplement_IGame();

        //Arrange
        IPlayer playerToPlay = _game.PlayerToPlayId == _playerNorthMock.Object.Id ? _playerNorthMock.Object : _playerSouthMock.Object;
        IPlayer opponent = _game.PlayerToPlayId == _playerNorthMock.Object.Id ? _playerSouthMock.Object : _playerNorthMock.Object;
        IMoveCard card = Random.Shared.NextItem(playerToPlay.MoveCards);
        ICoordinate to = opponent.School.TempleArchPosition;

        IPawn? capturedPawn;
        _playMatMock.Setup(p => p.ExecuteMove(It.IsAny<IMove>(), out capturedPawn)).Callback(
            (IMove move, out IPawn? innerCapturedPawn) =>
            {
                innerCapturedPawn = null;
                move.Pawn.Position = to;
            });

        //Act
        _game.MovePawn(playerToPlay.Id, playerToPlay.School.Master.Id, card.Name, to);

        //Assert
        _playMatMock.Verify(
            p => p.ExecuteMove(
                It.Is<IMove>(move =>
                    move.Pawn != null && 
                    move.Pawn.Id == playerToPlay.School.Master.Id && 
                    move.To == to &&
                    move.Card != null && 
                    move.Card == card), out capturedPawn), Times.Once,
            "The play mat is not used correctly to execute the move.");

        Assert.That(_game.WinnerPlayerId, Is.EqualTo(playerToPlay.Id),
                       "The player that made the winning move should be set as the winner.");
        Assert.That(_game.WinnerMethod, Does.Contain("way of the stream").IgnoreCase,
            "The winner method should be 'Way of the Stream'");
    }

    [MonitoredTest]
    public void MovePawn_WinningMoveByWayOfTheStone_ShouldSetWinner()
    {
        Class_ShouldImplement_IGame();

        //Arrange
        IPlayer playerToPlay = _game.PlayerToPlayId == _playerNorthMock.Object.Id ? _playerNorthMock.Object : _playerSouthMock.Object;
        IPlayer opponent = _game.PlayerToPlayId == _playerNorthMock.Object.Id ? _playerSouthMock.Object : _playerNorthMock.Object;
        IMoveCard card = Random.Shared.NextItem(playerToPlay.MoveCards);
        ICoordinate to = (new Coordinate(2,2) as ICoordinate)!;

        IPawn? capturedPawn;
        _playMatMock.Setup(p => p.ExecuteMove(It.IsAny<IMove>(), out capturedPawn)).Callback(
            (IMove move, out IPawn? innerCapturedPawn) =>
            {
                innerCapturedPawn = opponent.School.Master;
                move.Pawn.Position = to;
            });

        //Act
        _game.MovePawn(playerToPlay.Id, playerToPlay.School.Master.Id, card.Name, to);

        //Assert
        _playMatMock.Verify(
            p => p.ExecuteMove(
                It.Is<IMove>(move =>
                    move.Pawn != null &&
                    move.Pawn.Id == playerToPlay.School.Master.Id &&
                    move.To == to &&
                    move.Card != null &&
                    move.Card == card), out capturedPawn), Times.Once,
            "The play mat is not used correctly to execute the move.");

        Assert.That(_game.WinnerPlayerId, Is.EqualTo(playerToPlay.Id),
            "The player that made the winning move should be set as the winner.");
        Assert.That(_game.WinnerMethod, Does.Contain("way of the stone").IgnoreCase,
            "The winner method should be 'Way of the Stone'");
    }

    [MonitoredTest]
    public void SkipMovementAndExchangeCard_ItIsNotThePlayersTurn_ShouldThrowApplicationException()
    {
        Class_ShouldImplement_IGame();

        //Arrange
        Guid playerNotAtTurnId = _game.PlayerToPlayId == _playerNorthMock.Object.Id ? _playerSouthMock.Object.Id : _playerNorthMock.Object.Id;
        IMoveCard card = Random.Shared.NextItem(_playerNorthMock.Object.MoveCards);

        //Act + Assert
        Assert.That(() => _game.SkipMovementAndExchangeCard(playerNotAtTurnId, card.Name),
            Throws.InstanceOf<ApplicationException>().With.Message.Contains("turn").IgnoreCase);
    }

    [MonitoredTest]
    public void SkipMovementAndExchangeCard_ThereIsAPawnThatCanMakeAValidMove_ShouldThrowApplicationException()
    {
        Class_ShouldImplement_IGame();

        //Arrange
        IPlayer playerToPlay = _game.PlayerToPlayId == _playerNorthMock.Object.Id ? _playerNorthMock.Object : _playerSouthMock.Object;
        IMoveCard card = Random.Shared.NextItem(playerToPlay.MoveCards);

        _playMatMock.Setup(p => p.GetValidMoves(It.IsAny<IPawn>(), It.IsAny<IMoveCard>(), It.IsAny<Direction>()))
            .Returns([new MoveMockBuilder().Object]);

        //Act + Assert
        Assert.That(() => _game.SkipMovementAndExchangeCard(playerToPlay.Id, card.Name),
            Throws.InstanceOf<ApplicationException>().With.Message.Contains("valid move").IgnoreCase);
    }

    [MonitoredTest]
    public void SkipMovementAndExchangeCard_NoValidMovePossible_ShouldExchangeCard()
    {
        Class_ShouldImplement_IGame();

        //Arrange
        IPlayer playerToPlay = _game.PlayerToPlayId == _playerNorthMock.Object.Id ? _playerNorthMock.Object : _playerSouthMock.Object;
        IMoveCard card = Random.Shared.NextItem(playerToPlay.MoveCards);
        IMoveCard extraCard = _game.ExtraMoveCard;
        IPlayer opponent = (_game.Players[0].Id == playerToPlay.Id) ? _game.Players[1] : _game.Players[0];

        _playMatMock.Setup(p => p.GetValidMoves(It.IsAny<IPawn>(), It.IsAny<IMoveCard>(), It.IsAny<Direction>()))
            .Returns(new List<IMove>());

        //Act
        _game.SkipMovementAndExchangeCard(playerToPlay.Id, card.Name);

        //Assert
        _playMatMock.Verify(
            mat => mat.GetValidMoves(It.IsAny<IPawn>(), It.IsAny<IMoveCard>(), playerToPlay.Direction),
            Times.Exactly(5 * 2),
            "The PlayMat should be used to verify if none of the pawns of the player to play can make a move. Every pawn and every card of the player to play should be checked.");

        _playMatMock.Verify(
            mat => mat.GetValidMoves(It.Is<IPawn>(p => p.OwnerId == opponent.Id), It.IsAny<IMoveCard>(),
                playerToPlay.Direction), Times.Never,
            "Only the moves of the pawns of the player to play should be checked");

        Assert.That(_game.ExtraMoveCard, Is.EqualTo(card),
            "The extra move card should be the card that was played by the player to play.");

        Assert.That(playerToPlay.MoveCards, Has.One.Matches<IMoveCard>(c => c.Name == extraCard.Name),
            "The former extra card should now be one of the player's move cards.");

        Assert.That(_game.PlayerToPlayId, Is.EqualTo(opponent.Id),
            "The player to play should be set to the opponent after exchanging the card.");
    }
}