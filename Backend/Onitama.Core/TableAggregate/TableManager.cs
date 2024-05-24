using Microsoft.AspNetCore.Identity;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;
using System.Linq.Expressions;

namespace Onitama.Core.TableAggregate;

/// <inheritdoc cref="ITableManager"/>
internal class TableManager : ITableManager
{
    private ITableFactory _tableFactory;
    private ITableRepository _tableRepository;
    private IGameRepository _gameRepository;
    private IGameFactory _gameFactory;
    private IGamePlayStrategy _gamePlayStrategy;

    public TableManager(
        ITableRepository tableRepository, 
        ITableFactory tableFactory, 
        IGameRepository gameRepository,
        IGameFactory gameFactory,
        IGamePlayStrategy gamePlayStrategy)
    {
        this._gameFactory = gameFactory;
        this._tableRepository = tableRepository;
        this._gamePlayStrategy = gamePlayStrategy;
        this._tableFactory = tableFactory;
        this._gameRepository = gameRepository;
    }

    public ITable AddNewTableForUser(User user, TablePreferences preferences)
    {
        var table = _tableFactory.CreateNewForUser(user, preferences);
        _tableRepository.Add(table);
        return table;
    }

    public ITable AddCompTableForUser(User user, CompTablePreferences preferences)
    {
        var table = _tableFactory.CreateCompForUser(user, preferences);
        _tableRepository.Add(table);
        return table;
    }

    public ITable AddBlitzTableForUser(User user, BlitzTablePreferences preferences)
    {
        var table = _tableFactory.CreateBlitzForUser(user, preferences);
        _tableRepository.Add(table);
        return table;
    }

    public ITable AddCustomTableForUser(User user, CustomTablePreferences preferences)
    {
        var table = _tableFactory.CreateCustomForUser(user, preferences);
        _tableRepository.Add(table);
        return table;
    }

    public void JoinTable(Guid tableId, User user)
    {
        var table = _tableRepository.Get(tableId);
        if (table.HasAvailableSeat)
        {
            table.Join(user);
        }
        
    }

    public void LeaveTable(Guid tableId, User user)
    {
        var table = _tableRepository.Get(tableId);
        table.Leave(user.Id);
        if(table.SeatedPlayers.Count < 1)
        {
            _tableRepository.Remove(tableId);
        }
    }

    public void FillWithArtificialPlayers(Guid tableId, User user)
    {
        var table = _tableRepository.Get(tableId);
        if(table.OwnerPlayerId != user.Id)
        {
            throw new InvalidOperationException("You are not the owner of this table");
        }

        int depth = 0;

        if (table.Preferences.TableType == "ai-medium")
        {
            depth = 2;
        }

        else if (table.Preferences.TableType == "ai-hard")
        {
            depth = 5;
        }

        else if (table.Preferences.TableType == "ai-extreme")
        {
            depth = 15;
        }

        var evaluator = new GameEvaluator();
        var strategy = new MiniMaxGamePlayStrategy(evaluator, 2);
        table.FillWithArtificialPlayers(strategy);
    }

    public IGame StartGameForTable(Guid tableId, User user)
    {
        var table = _tableRepository.Get(tableId);
        if (table.HasAvailableSeat)
        {
            throw new InvalidOperationException("There are not enough players at this table to start the game.");
        }
        if(user.Id != table.OwnerPlayerId)
        {
            throw new InvalidOperationException("Only the owner can start the game at this table.");
        }
        var game = _gameFactory.CreateNewForTable(table);
        _gameRepository.Add(game);
        table.GameId = game.Id;
        return game;
    }
    public IGame StartGameForTableAi(Guid tableId, User user)
    {
        var table = _tableRepository.Get(tableId);
        if (table.HasAvailableSeat)
        {
            throw new InvalidOperationException("There are not enough players at this table to start the game.");
        }
        if (user.Id != table.OwnerPlayerId)
        {
            throw new InvalidOperationException("Only the owner can start the game at this table.");
        }
        var game = _gameFactory.CreateNewForTable(table);
        foreach (var oldGame in _gameRepository.GetAll())
        {
            foreach (var player in oldGame.Players)
            {
                if (player.Id == user.Id)
                {
                    foreach (var otherPlayer in oldGame.Players)
                    {
                        if (otherPlayer.Id != user.Id)
                        {
                            oldGame.UpdateWinner(otherPlayer.Id);
                        }
                    }
                }
            }
        }
        _gameRepository.Add(game);
        table.GameId = game.Id;
        return game;
    }
}