using System.Diagnostics;
using System.Drawing;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.Util;

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
        var playMat = new PlayMat(5);
        var players = new IPlayer[table.SeatedPlayers.Count];
        table.SeatedPlayers.CopyTo(players, 0);
        Game game = new Game(Guid.NewGuid(), playMat, players, moveCard.ElementAt(0));
        foreach (var player in players)
        {
            game.PlayMat.PositionSchoolOfPlayer(player);
        }
        
        return game;
    }
}