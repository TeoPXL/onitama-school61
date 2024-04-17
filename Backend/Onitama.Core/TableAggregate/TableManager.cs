using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;

namespace Onitama.Core.TableAggregate;

/// <inheritdoc cref="ITableManager"/>
internal class TableManager : ITableManager
{
    public TableManager(
        ITableRepository tableRepository, 
        ITableFactory tableFactory, 
        IGameRepository gameRepository,
        IGameFactory gameFactory,
        IGamePlayStrategy gamePlayStrategy)
    {
       
    }

    public ITable AddNewTableForUser(User user, TablePreferences preferences)
    {
        var table = new TableFactory().CreateNewForUser(user, preferences);
        return table;
    }

    public void JoinTable(Guid tableId, User user)
    {
        throw new NotImplementedException();
    }

    public void LeaveTable(Guid tableId, User user)
    {
        throw new NotImplementedException();
    }

    public void FillWithArtificialPlayers(Guid tableId, User user)
    {
        throw new NotImplementedException();
    }

    public IGame StartGameForTable(Guid tableId, User user)
    {
        throw new NotImplementedException();
    }
}