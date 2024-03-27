using System.Security.Claims;
using AutoMapper;
using Guts.Client.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Moq;
using Onitama.Api.Controllers;
using Onitama.Api.Models.Input;
using Onitama.Api.Models.Output;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.Tests.Builders;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util.Contracts;

namespace Onitama.Api.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Onitama", "GamesCtlr",
        @"Onitama.Api\Controllers\GamesController.cs")]
    public class GamesControllerTests
    {
        private Mock<IGameService> _gameServiceMock = null!;
        private Mock<ICoordinateFactory> _coordinateFactoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private GamesController _controller = null!;
        private User _loggedInUser = null!;

        [SetUp]
        public void SetUp()
        {
            _gameServiceMock = new Mock<IGameService>();

            _coordinateFactoryMock = new Mock<ICoordinateFactory>();
            _coordinateFactoryMock.Setup(f => f.Create(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int row, int column) => new CoordinateMockBuilder(row, column).Object);

            _mapperMock = new Mock<IMapper>();
            _controller = new GamesController(_gameServiceMock.Object, _coordinateFactoryMock.Object, _mapperMock.Object);

            _loggedInUser = new UserBuilder().Build();
            var userClaimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, _loggedInUser.Id.ToString())
                })
            );
            var context = new ControllerContext { HttpContext = new DefaultHttpContext() };
            context.HttpContext.User = userClaimsPrincipal;
            _controller.ControllerContext = context;
        }

        [MonitoredTest]
        public void GetGame_ShouldUseServiceToRetrieveGame()
        {
            // Arrange
            IGame game = new GameMockBuilder().Object;
            var gameModel = new GameModel();
            _gameServiceMock.Setup(s => s.GetGame(game.Id)).Returns(game);
            _mapperMock.Setup(m => m.Map<GameModel>(game)).Returns(gameModel);

            // Act
            var result = _controller.GetGame(game.Id) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
            _mapperMock.Verify(mapper => mapper.Map<GameModel>(game), Times.Once,
                "The game is not correctly mapped to a game model");
            Assert.That(result.Value, Is.SameAs(gameModel), "The mapped game model is not in the OkObjectResult");
        }

        [MonitoredTest]
        public void GetPossibleMovesForGame_ShouldUseServiceToRetrievePossibleMoves()
        {
            // Arrange
            Guid gameId = Guid.NewGuid();
            Guid pawnId = Guid.NewGuid();
            string moveCardName = Guid.NewGuid().ToString();
            
            var moves = new List<IMove>()
            {
                new MoveMockBuilder().Object,
                new MoveMockBuilder().Object
            };
            _gameServiceMock.Setup(s => s.GetPossibleMovesForPawn(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(moves);
            _mapperMock.Setup(m => m.Map<MoveModel>(It.IsAny<IMove>())).Returns(new MoveModel());

            // Act
            var result = _controller.GetPossibleMovesForPawn(gameId, pawnId, moveCardName) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
            _gameServiceMock.Verify(service => service.GetPossibleMovesForPawn(gameId, _loggedInUser.Id, pawnId, moveCardName), Times.Once,
                               "The service is not called with the correct parameters");
            _mapperMock.Verify(mapper => mapper.Map<MoveModel>(It.IsIn<IMove>(moves)), Times.Exactly(moves.Count),
                               "The possible moves are not correctly mapped to a possible moves model");
            Assert.That(result!.Value, Is.InstanceOf<IEnumerable<MoveModel>>(), "The mapped possible moves are not in the OkObjectResult");
        }

        [MonitoredTest]
        public void MovePawn_ShouldUseServiceToMovePawn()
        {
            // Arrange
            Guid gameId = Guid.NewGuid();
            var inputModel = new MovePawnModel()
            {
                PawnId = Guid.NewGuid(),
                MoveCardName = Guid.NewGuid().ToString(),
                To = new CoordinateModel()
                {
                    Row = Random.Shared.Next(0,5),
                    Column = Random.Shared.Next(0, 5)
                }
            };

            // Act
            var result = _controller.MovePawn(gameId, inputModel) as OkResult;

            // Assert
            Assert.That(result, Is.Not.Null, "An instance of 'OkResult' should be returned.");
            _coordinateFactoryMock.Verify(f => f.Create(inputModel.To.Row, inputModel.To.Column),
                "The 'to' coordinate should be created correctly using the coordinate factory");

            _gameServiceMock.Verify(
                service => service.MovePawn(
                    gameId, 
                    _loggedInUser.Id, 
                    inputModel.PawnId, 
                    inputModel.MoveCardName,
                    It.Is<ICoordinate>(c => c.Row == inputModel.To.Row && c.Column == inputModel.To.Column)),
                Times.Once,
                "The service is not called with the correct parameters");
        }

        [MonitoredTest]
        public void SkipMovement_ShouldUseServiceToSkipMovement()
        {
            // Arrange
            Guid gameId = Guid.NewGuid();
            var inputModel = new SkipMovementModel()
            {
                MoveCardName = Guid.NewGuid().ToString()
            };

            // Act
            var result = _controller.SkipMovement(gameId, inputModel) as OkResult;

            // Assert
            Assert.That(result, Is.Not.Null, "An instance of 'OkResult' should be returned.");
            _gameServiceMock.Verify(
                service => service.SkipMovementAndExchangeCard(gameId, _loggedInUser.Id, inputModel.MoveCardName),
                Times.Once, "The service is not called with the correct parameters");
        }
    }
}
