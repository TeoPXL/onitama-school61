using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Microsoft.AspNetCore.Identity;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;
using static Onitama.Core.TableAggregate.Table;

namespace Onitama.Core.GameAggregate;

internal class GameFactory : IGameFactory
{
    private IMoveCardRepository _moveCardRepository;
    private static Random _random = new Random();

    public GameFactory(IMoveCardRepository moveCardRepository)
    {
        this._moveCardRepository = moveCardRepository;
    }

    public IGame CreateNewForTable(ITable table)
    {
        Color[] colors = table.SeatedPlayers.Select(p => p.Color).ToArray();
        
        IMoveCard[] moveCards;
        IMoveCard[] wotwCards = [];
        if (table.Preferences.MoveCardSet == MoveCardSet.Custom)
        {
            moveCards = _moveCardRepository.LoadSetCustom(table.Preferences.MoveCardSet, colors, table.Preferences.MoveCardString);
        } else
        {
            moveCards = _moveCardRepository.LoadSet(table.Preferences.MoveCardSet, colors);
        }
        if (table.Preferences.TableType == "wotw")
        {
            wotwCards = _moveCardRepository.LoadSetCustom(MoveCardSet.WayOfTheWind, colors, table.Preferences.MoveCardString);
        }
        moveCards = moveCards.OrderBy(card => _random.Next()).ToArray();
        wotwCards = wotwCards.OrderBy(card => _random.Next()).ToArray();
        //throw new Exception(table.Preferences.MoveCardSet.ToString());
        var playMat = new PlayMat(5);
        if (table.Preferences.TableType == "wotw")
        {
            table.SeatedPlayers[0].MoveCards.Add(wotwCards[0]);
            table.SeatedPlayers[0].MoveCards.Add(moveCards[1]);
            table.SeatedPlayers[1].MoveCards.Add(wotwCards[2]);
            table.SeatedPlayers[1].MoveCards.Add(moveCards[3]);
        } else
        {
            table.SeatedPlayers[0].MoveCards.Add(moveCards[0]);
            table.SeatedPlayers[0].MoveCards.Add(moveCards[1]);
            table.SeatedPlayers[1].MoveCards.Add(moveCards[2]);
            table.SeatedPlayers[1].MoveCards.Add(moveCards[3]);
        }
        
        var players = new IPlayer[table.SeatedPlayers.Count];
        table.SeatedPlayers.CopyTo(players, 0);
        string gameType = table.Preferences.TableType;
        Game game = new Game(Guid.NewGuid(), playMat, players, moveCards.ElementAt(moveCards.Length - 1), gameType);
        foreach (var player in players)
        {
            var pawns = new IPawn[5];
            pawns[0] = new Pawn(Guid.NewGuid(), player.Id, SchoolAggregate.Contracts.PawnType.Student);
            pawns[1] = new Pawn(Guid.NewGuid(), player.Id, SchoolAggregate.Contracts.PawnType.Student);
            pawns[2] = new Pawn(Guid.NewGuid(), player.Id, SchoolAggregate.Contracts.PawnType.Master);
            pawns[3] = new Pawn(Guid.NewGuid(), player.Id, SchoolAggregate.Contracts.PawnType.Student);
            pawns[4] = new Pawn(Guid.NewGuid(), player.Id, SchoolAggregate.Contracts.PawnType.Student);
            player.SetSchool(new School(pawns));

            game.PlayMat.PositionSchoolOfPlayer(player);
        }

        if (table.Preferences.TableType == "wotw")
        {
            var spirit = new Pawn(Guid.NewGuid(), Guid.Parse("00000000000000000000000000000001"), SchoolAggregate.Contracts.PawnType.Spirit);
            game.PlayMat.PositionSpirit(spirit);
        }

        game.checkValidMoves();
        
        return game;
    }
}