using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Onitama.Api.Models;
using Onitama.Api.Models.Input;
using Onitama.Api.Models.Output;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

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
    }
}
