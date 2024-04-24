using System.Diagnostics;
using System.Drawing;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.TableAggregate.Contracts;

namespace Onitama.Core.GameAggregate;

internal class GameFactory : IGameFactory
{
    private IMoveCardRepository _moveCardRepository;
    public GameFactory(IMoveCardRepository moveCardRepository)
    {
       this._moveCardRepository = moveCardRepository;
    }

    public IGame CreateNewForTable(ITable table)
    {
        var moveCard = _moveCardRepository.LoadSet(table.Preferences.MoveCardSet, table.GetUsedColors());
        
        table.GameId = Guid.NewGuid();
        /// <warning>
        /// This Game constructor is temprary. It needs to implement PlayMat, but at the moment I don't know how.
        /// </warning>
        Game game = new Game(table.GameId, table.GetSeatedPlayers(), moveCard.ElementAt(0));
        
        return game;
    }
}