﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Onitama.Api.Models;
using Onitama.Api.Models.Input;
using Onitama.Api.Models.Output;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using Onitama.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Onitama.Api.Controllers
{
    /// <summary>
    /// Provides game-play functionality.
    /// </summary>
    [Route("api/[controller]")]
    public class GamesController : ApiControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ICoordinateFactory _coordinateFactory;
        private readonly IMapper _mapper;

        public GamesController(IGameService gameService, ICoordinateFactory coordinateFactory, IMapper mapper)
        {
            _gameService = gameService;
            _coordinateFactory = coordinateFactory;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets information about your game
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GameModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult GetGame(Guid id)
        {
            IGame game = _gameService.GetGame(id);
            GameModel gameModel = _mapper.Map<GameModel>(game);
            return Ok(gameModel);
        }

        /// <summary>
        /// Gets every active game that does not have a winner
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IReadOnlyList<IGame>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllGames()
        {
            List<IGame> games = _gameService.GetAllGames();
            List<GameModel> models = games
                .Where(game => game.WinnerPlayerId == Guid.Empty)
                .Select(game => _mapper.Map<GameModel>(game))
                .ToList();
            return Ok(models);
        }

        /// <summary>
        /// Gets the possible moves of a certain pawn for a certain move card.
        /// The pawn must be owned by the player associated with the authenticated user.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <param name="pawnId">Id (guid) of the pawn that possibly is going to make a move</param>
        /// <param name="moveCardName">The name of the card this possibly is going to be used</param>
        [HttpGet("{id}/possible-moves-of/{pawnId}/for-card/{moveCardName}")]
        [ProducesResponseType(typeof(IReadOnlyList<IMove>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPossibleMovesForPawn(Guid id, Guid pawnId, string moveCardName)
        {
            IReadOnlyList<IMove> moves = _gameService.GetPossibleMovesForPawn(id, UserId, pawnId, moveCardName);
            List<MoveModel> models = moves.Select(move => _mapper.Map<MoveModel>(move)).ToList();
            return Ok(models);
        }

        /// <summary>
        /// Moves a pawn for the player associated with the authenticated user.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <param name="inputModel">
        /// Information about the move the player wants to make.
        /// </param>
        [HttpPost("{id}/move-pawn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult MovePawn(Guid id, [FromBody] MovePawnModel inputModel)
        {
            ICoordinate to = _coordinateFactory.Create(inputModel.To.Row, inputModel.To.Column);
            _gameService.MovePawn(id, UserId, inputModel.PawnId, inputModel.MoveCardName, to);

            return Ok();
        }

        [HttpPost("{id}/move-pawn-ai")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MovePawnAi(Guid id, [FromBody] MovePawnModel inputModel)
        {
            ICoordinate to = _coordinateFactory.Create(inputModel.To.Row, inputModel.To.Column);
            _gameService.MovePawnAi(id, UserId, inputModel.PawnId, inputModel.MoveCardName, to);
            await Task.Delay(2000);
            _gameService.MakeAIMove(id);

            return Ok();
        }

        [HttpPost("{id}/move-pawn-wotw")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MovePawnWotw(Guid id, [FromBody] MovePawnModelWotw inputModel)
        {
            ICoordinate to = _coordinateFactory.Create(inputModel.To.Row, inputModel.To.Column);
            ICoordinate spiritTo = _coordinateFactory.Create(inputModel.SpiritTo.Row, inputModel.SpiritTo.Column);
            _gameService.MovePawnAi(id, UserId, inputModel.PawnId, inputModel.MoveCardName, to, "wotw");
            if(_gameService.GetGame(id).WinnerPlayerId == Guid.Empty){
                await Task.Delay(1000);
                _gameService.MovePawnAi(id, UserId, inputModel.SpiritId, inputModel.MoveCardName, spiritTo, "default");
            }

            return Ok();
        }

        /// <summary>
        /// Moves a pawn for the player associated with the authenticated user.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <param name="inputModel">
        /// Information about the move the player wants to make.
        /// </param>
        [HttpPost("{id}/move-pawn-competitive")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MovePawnCompetitive(Guid id, [FromBody] MovePawnModel inputModel, [FromServices] IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<OnitamaDbContext>();
            ICoordinate to = _coordinateFactory.Create(inputModel.To.Row, inputModel.To.Column);
            _gameService.MovePawnAi(id, UserId, inputModel.PawnId, inputModel.MoveCardName, to);
            var game = _gameService.GetGame(id);
            var players = game.Players;
            if(game.WinnerPlayerId != Guid.Empty)
            {
                if (dbContext != null)
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        dbContext.Entry(players[i].User).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
            return Ok();
        }

        /// <summary>
        /// States that the player associated with the authenticated user wants to skip their movement and only exchange a card.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <param name="inputModel">
        /// Information about the card you want to exchange.
        /// </param>
        [HttpPost("{id}/skip-movement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult SkipMovement(Guid id, [FromBody] SkipMovementModel inputModel)
        {
            _gameService.SkipMovementAndExchangeCard(id, UserId, inputModel.MoveCardName);
            return Ok();
        }

        [HttpPost("{id}/skip-movement-ai")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SkipMovementAi(Guid id, [FromBody] SkipMovementModel inputModel)
        {
            _gameService.SkipMovementAndExchangeCard(id, UserId, inputModel.MoveCardName);
            await Task.Delay(2000);
            _gameService.MakeAIMove(id);
            return Ok();
        }
    }
}
