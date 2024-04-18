using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.GameAggregate;

internal class GameService : IGameService
{
    private IGameRepository _gameRepository;
    public GameService(IGameRepository gameRepository)
    {
        this._gameRepository = gameRepository;
    }

    public IGame GetGame(Guid gameId)
    {
        return _gameRepository.GetById(gameId);
        //throw new NotImplementedException("TODO: use the constructor-injected repository to retrieve a game");
    }

    public IReadOnlyList<IMove> GetPossibleMovesForPawn(Guid gameId, Guid playerId, Guid pawnId, string moveCardName)
    {
        return _gameRepository.GetById(gameId).GetPossibleMovesForPawn(playerId, pawnId, moveCardName);
        //throw new NotImplementedException(
        //    "TODO: use the constructor-injected repository to retrieve the game and get the possible moves of that game");
    }

    public void MovePawn(Guid gameId, Guid playerId, Guid pawnId, string moveCardName, ICoordinate to)
    {
        _gameRepository.GetById(gameId).MovePawn(playerId, pawnId, moveCardName, to);
        //throw new NotImplementedException(
        //    "TODO: use the constructor-injected repository to retrieve the game and get move a pawn of that game");
    }

    public void SkipMovementAndExchangeCard(Guid gameId, Guid playerId, string moveCardName)
    {
        _gameRepository.GetById(gameId).SkipMovementAndExchangeCard(playerId, moveCardName);
        //throw new NotImplementedException(
        //    "TODO: use the constructor-injected repository to retrieve the game and skip a movement for that game");
    }
}