using Microsoft.AspNetCore.Identity;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.UserAggregate;
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
    }

    public IReadOnlyList<IMove> GetPossibleMovesForPawn(Guid gameId, Guid playerId, Guid pawnId, string moveCardName)
    {
        return _gameRepository.GetById(gameId).GetPossibleMovesForPawn(playerId, pawnId, moveCardName);
    }

    public void MovePawn(Guid gameId, Guid playerId, Guid pawnId, string moveCardName, ICoordinate to)
    {
        _gameRepository.GetById(gameId).MovePawn(playerId, pawnId, moveCardName, to);
    }

    public List<IGame> GetAllGames()
    {
        return _gameRepository.GetAll();
    }

    public void SkipMovementAndExchangeCard(Guid gameId, Guid playerId, string moveCardName)
    {
        _gameRepository.GetById(gameId).SkipMovementAndExchangeCard(playerId, moveCardName);
    }

    public void MakeAIMove(Guid gameId)
    {
        _gameRepository.GetById(gameId).MakeAIMove();
    }
}