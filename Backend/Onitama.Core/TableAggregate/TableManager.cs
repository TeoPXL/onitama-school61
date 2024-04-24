﻿using Onitama.Core.GameAggregate.Contracts;
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
        var table =_tableRepository.Get(tableId);
        table.Leave(user.Id);
        if(table.SeatedPlayers.Count == 0)
        {
            _tableRepository.Remove(tableId);
        }
    }

    public void FillWithArtificialPlayers(Guid tableId, User user)
    {
        throw new NotImplementedException();
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
        IGame game = _gameFactory.CreateNewForTable(table);
        table.GameId = game.Id;
        _gameRepository.Add(game);
        return game;
    }
}