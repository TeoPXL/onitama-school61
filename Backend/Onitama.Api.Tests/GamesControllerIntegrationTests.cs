using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Guts.Client.Core;
using Guts.Client.Core.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Onitama.Api.Controllers;
using Onitama.Api.Models.Input;
using Onitama.Api.Models.Output;
using Onitama.Api.Util;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.TableAggregate;
using Onitama.Core.Tests.Builders;
using Onitama.Core.Tests.Extensions;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;
using Onitama.Infrastructure;

namespace Onitama.Api.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "GamesIntegration",
    @"Onitama.Api\Controllers\GamesController.cs;
Onitama.Core\GameAggregate\GameService.cs;
Onitama.Core\GameAggregate\GameFactory.cs;
Onitama.Core\GameAggregate\Game.cs;
Onitama.Core\GameAggregate\Move.cs;")]
public class GamesControllerIntegrationTests : ControllerIntegrationTestsBase<GamesController>
{
    [MonitoredTest]
    public void GetGame_JustAfterCreation_ShouldReturnAGameWithACorrectStartSituation()
    {
        TableModel table = StartANewGameForANewTable();

        GameModel? game = GetGame(ClientA, table.GameId);
        Assert.That(game!.Id, Is.EqualTo(table.GameId), "The returned game has an incorrect Id");

        Assert.That(game.PlayMat, Is.Not.Null, "The game should have a play mat");
        bool playMatHasCorrectDimensions = game.PlayMat.Grid.GetLength(0) == 5 && game.PlayMat.Grid.GetLength(1) == 5;
        Assert.That(playMatHasCorrectDimensions, Is.True, "The play mat of the game should have a 5x5 grid");

        Assert.That(game.Players.Length, Is.EqualTo(2), "The game should have 2 players");

        PlayerModel? playerThatShouldStart = game.Players.FirstOrDefault(p => p.Color == game.ExtraMoveCard.StampColor);
        Assert.That(playerThatShouldStart, Is.Not.Null,
            "No player found that has the same color as the stamp of the extra move card");
        Assert.That(game.PlayerToPlayId, Is.EqualTo(playerThatShouldStart!.Id),
            $"The player to start has a different color than the stamp color of the extra move card");

        PlayerModel? playerNorth = game.Players.FirstOrDefault(p => p.Direction == Direction.North);
        Assert.That(playerNorth, Is.Not.Null, "No player found that has direction North");
        Assert.That(playerNorth!.MoveCards.Length, Is.EqualTo(2), "Each player should have 2 move cards");
        AssertPlayerSchool(playerNorth, "bottom", game.PlayMat);

        PlayerModel? playerSouth = game.Players.FirstOrDefault(p => p.Direction == Direction.South);
        Assert.That(playerSouth, Is.Not.Null, "No player found that has direction South");
        Assert.That(playerSouth!.MoveCards.Length, Is.EqualTo(2), "Each player should have 2 move cards");
        AssertPlayerSchool(playerNorth, "top", game.PlayMat);

        Assert.That(game.ExtraMoveCard, Is.Not.Null, "The game should have an extra move card");

        Assert.That(game.WinnerPlayerId, Is.EqualTo(Guid.Empty), "The game should not have a winner yet");
    }

    private static void AssertPlayerSchool(PlayerModel player, string expectedRowPosition, PlayMatModel playMat)
    {
        Assert.That(player!.School.AllPawns.Length, Is.EqualTo(5),
            $"The school of the player with direction '{player.Direction}' should have 5 pawns");
        Assert.That(player.School.AllPawns.All(p => p.Position is not null && p.Position.Row == 0), Is.True,
            $"All the pawns of the school of the player with direction '{player.Direction}' should be positioned on the {expectedRowPosition} row");
        Assert.That(player.School.AllPawns.All(p => playMat.Grid[p.Position.Row, p.Position.Column] is not null
                                                    && playMat.Grid[p.Position.Row, p.Position.Column].Id == p.Id), Is.True,
            $"All the pawns of the school should also be found on the play mat of the game");

        PawnModel? masterNorth = player!.School.AllPawns.FirstOrDefault(p => p.Type == PawnType.Master);
        Assert.That(masterNorth, Is.Not.Null,
            $"No master pawn found for the player with direction '{player.Direction}'");
        Assert.That(masterNorth!.Position.Column, Is.EqualTo(2),
            $"The master pawn for the player with direction '{player.Direction}' should be in the middle column");
    }

    [MonitoredTest]
    public void HappyFlow_PlaySomeRandomMoves()
    {
        TableModel table = StartANewGameForANewTable();

        //Get game for user A
        GameModel? game = GetGame(ClientA, table.GameId);
        Assert.That(game!.Id, Is.EqualTo(table.GameId), "Error retrieving the game.\n The returned game has an incorrect Id");

        int moveCount = 0;
        while (moveCount < 30 && game.WinnerPlayerId == Guid.Empty)
        {
            game = PlayRandomMove(game);
            moveCount++;
        }

        string gameJson = JsonSerializer.Serialize(game, JsonSerializerOptions);

        if (game.WinnerPlayerId == Guid.Empty)
        {
            Assert.Pass($"Game after {moveCount} moves:\n Undecided\n\nGame(json):\n{gameJson}");
        }
        else
        {
            PlayerModel winner = game.Players.First(p => p.Id == game.WinnerPlayerId);
            Assert.Pass(
                $"Game after {moveCount} moves:\n {winner.Name} wins by '{game.WinnerMethod}'\n\nGame(json):\n{gameJson}");
        }
    }

    [MonitoredTest]
    public void MakeAnInvalidMove_ShouldReturn_400_BadRequest()
    {
        TableModel table = StartANewGameForANewTable();

        //Get game for user A
        GameModel? game = GetGame(ClientA, table.GameId);
        Assert.That(game!.Id, Is.EqualTo(table.GameId), "Error retrieving the game.\n The returned game has an incorrect Id");

        int numberOfInvalidMoves = 5;
        for (int i = 0; i < numberOfInvalidMoves; i++)
        {
            game = PlayInvalidMove(game);
        }
    }

    private GameModel PlayRandomMove(GameModel game)
    {
        PlayerModel? playerToPlay = game.Players.FirstOrDefault(p => p.Id == game.PlayerToPlayId);
        Assert.That(playerToPlay, Is.Not.Null,
            $"Player to play '{game.PlayerToPlayId}' not found in the game model retrieved from the Api");
        HttpClient clientToPlay = playerToPlay.Id == WarriorAAccessPass.User.Id ? ClientA : ClientB;

        //Get the possible moves for player
        PawnModel[] allPawnsAlive = playerToPlay.School.AllPawns.Where(p => p.Position is not null).ToArray();
        Assert.That(allPawnsAlive.Length, Is.GreaterThan(0),
            "No pawns that are positioned on the play mat found in the game model retrieved from the Api");
        Random.Shared.Shuffle(allPawnsAlive);
        int pawnIndex = 0;
        MoveModel? selectedMove = null;

        while (selectedMove is null && pawnIndex < allPawnsAlive.Length)
        {
            PawnModel pawn = allPawnsAlive[pawnIndex];

            var allPossibleMovesForPawn = new List<MoveModel>();
            foreach (MoveCardModel moveCard in playerToPlay.MoveCards)
            {
                List<MoveModel> movesForPawnAndCard = GetMovesForPawnAndCard(game, clientToPlay, pawn, moveCard);
                allPossibleMovesForPawn.AddRange(movesForPawnAndCard);
            }

            if (allPossibleMovesForPawn.Any())
            {
                int moveIndex = Random.Shared.Next(0, allPossibleMovesForPawn.Count);
                selectedMove = allPossibleMovesForPawn[moveIndex];
            }
            pawnIndex++;
        }

        if (selectedMove is null)
        {
            //skip movement
            int cardIndex = Random.Shared.Next(0, playerToPlay.MoveCards.Length);
            string cardNameToExchange = playerToPlay.MoveCards[cardIndex].Name;

            TestContext.WriteLine(
                $"Player '{playerToPlay.Name}' - " +
                $"Skip movement, exchange the '{cardNameToExchange}' card");

            HttpResponseMessage? result = clientToPlay.PostAsJsonAsync($"api/games/{game.Id}/skip-movement", new SkipMovementModel
            {
                MoveCardName = cardNameToExchange
            }).Result;

            ErrorModel? error = null;
            if (!result.IsSuccessStatusCode)
            {
                error = result.Content.ReadFromJsonAsync<ErrorModel>(JsonSerializerOptions).Result;
            }
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"Error skipping movement. {error?.Message}");
        }
        else
        {
            TestContext.WriteLine(
                $"Player '{playerToPlay.Name}' - " +
                $"Move pawn '{selectedMove.Pawn.Id}' from ({selectedMove.Pawn.Position.Row}, {selectedMove.Pawn.Position.Column}) " +
                $"to ({selectedMove.To.Row}, {selectedMove.To.Column}) using the '{selectedMove.Card.Name}' card");
            HttpResponseMessage? result = clientToPlay.PostAsJsonAsync($"api/games/{game.Id}/move-pawn", new MovePawnModel
            {
                MoveCardName = selectedMove.Card.Name,
                PawnId = selectedMove.Pawn.Id,
                To = selectedMove.To
            }).Result;

            ErrorModel? error = null;
            if (!result.IsSuccessStatusCode)
            {
                error = result.Content.ReadFromJsonAsync<ErrorModel>(JsonSerializerOptions).Result;
            }
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"Error moving pawn. {error?.Message}");
        }

        GameModel? modifiedGame = GetGame(clientToPlay, game.Id);
        Assert.That(modifiedGame!.PlayerToPlayId, Is.Not.EqualTo(game.PlayerToPlayId), "Player to play has not changed after move");
        Assert.That(modifiedGame.ExtraMoveCard, Is.Not.EqualTo(game.ExtraMoveCard), "Extra move card has not changed after move");
        return modifiedGame;
    }

    private GameModel PlayInvalidMove(GameModel game)
    {
        PlayerModel? playerToPlay = game.Players.FirstOrDefault(p => p.Id == game.PlayerToPlayId);
        Assert.That(playerToPlay, Is.Not.Null,
            $"Player to play '{game.PlayerToPlayId}' not found in the game model retrieved from the Api");
        HttpClient clientToPlay = playerToPlay.Id == WarriorAAccessPass.User.Id ? ClientA : ClientB;

        //Get the possible moves for player
        PawnModel[] allPawnsAlive = playerToPlay.School.AllPawns.Where(p => p.Position is not null).ToArray();
        Assert.That(allPawnsAlive.Length, Is.GreaterThan(0),
            "No pawns that are positioned on the play mat found in the game model retrieved from the Api");

        PawnModel pawn = Random.Shared.NextItem(allPawnsAlive);
        MoveCardModel moveCard = Random.Shared.NextItem(playerToPlay.MoveCards);
        List<MoveModel> movesForPawnAndCard = GetMovesForPawnAndCard(game, clientToPlay, pawn, moveCard);

        Assert.That(movesForPawnAndCard.Count, Is.LessThan(20), $"It is impossible that a pawn has 20 or more possible moves");

        //Create a move that is not valid
        int row = Random.Shared.Next(0,5);
        int column = Random.Shared.Next(0, 5);
        bool invalidTargetFound = false;

        while (!invalidTargetFound)
        {
            if (movesForPawnAndCard.All(m => m.To.Row != row || m.To.Column != column))
            {
                invalidTargetFound = true;
            }
            else
            {
                row = Random.Shared.Next(0, 5);
                column = Random.Shared.Next(0, 5);
            }
        }

        Assert.That(invalidTargetFound, Is.True, $"Could not find an invalid target for pawn '{pawn.Id}' and card '{moveCard.Name}'");

        var movePawnModel = new MovePawnModel
        {
            MoveCardName = moveCard.Name,
            PawnId = pawn.Id,
            To = new CoordinateModel { Row = row, Column = column }
        };

        TestContext.WriteLine(
            $"Player '{playerToPlay.Name}' - " +
            $"Move pawn '{movePawnModel.PawnId}' from ({pawn.Position.Row}, {pawn.Position.Column}) " +
            $"to ({movePawnModel.To.Row}, {movePawnModel.To.Column}) using the '{movePawnModel.MoveCardName}' card");
        HttpResponseMessage? result = clientToPlay.PostAsJsonAsync($"api/games/{game.Id}/move-pawn", movePawnModel).Result;

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "An invalid move should return a 400 BadRequest status code");
        ErrorModel? error = result.Content.ReadFromJsonAsync<ErrorModel>(JsonSerializerOptions).Result;
        Assert.That(error, Is.Not.Null, "An invalid move should return an error message " +
                                        "(you can achieve this by throwing an ApplicationException or an InvalidOperationException)");


        GameModel? modifiedGame = GetGame(clientToPlay, game.Id);
        Assert.That(modifiedGame!.PlayerToPlayId, Is.EqualTo(game.PlayerToPlayId), "Player to play should not change after an invalid move");
        Assert.That(modifiedGame.ExtraMoveCard, Is.EqualTo(game.ExtraMoveCard), "Extra move card should not change after an invalid move");
        return modifiedGame;
    }

    private GameModel GetGame(HttpClient client, Guid gameId)
    {
        var result = client.GetAsync($"api/games/{gameId}").Result;
        ErrorModel? error = null;
        if (!result.IsSuccessStatusCode)
        {
            error = result.Content.ReadFromJsonAsync<ErrorModel>(JsonSerializerOptions).Result;
        }
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
        $"Error retrieving the game with id '{gameId}'.\n {error?.Message}");

        GameModel? gameModel = result.Content.ReadFromJsonAsync<GameModel>(JsonSerializerOptions).Result;
        Assert.That(gameModel, Is.Not.Null, $"Error retrieving the game with id '{gameId}'.\n Game not found in Api response");
        return gameModel!;
    }

    private List<MoveModel> GetMovesForPawnAndCard(GameModel game, HttpClient clientToPlay, PawnModel pawn, MoveCardModel moveCard)
    {
        var result = clientToPlay.GetAsync($"api/games/{game.Id}/possible-moves-of/{pawn.Id}/for-card/{moveCard.Name}").Result;
        ErrorModel? error = null;
        if (!result.IsSuccessStatusCode)
        {
            error = result.Content.ReadFromJsonAsync<ErrorModel>(JsonSerializerOptions).Result;
        }

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"Error retrieving possible moves (pawn='{pawn.Id}', card='{moveCard.Name}').\n {error?.Message}");

        List<MoveModel>? movesForPawnAndCard = result.Content.ReadFromJsonAsync<List<MoveModel>>(JsonSerializerOptions).Result;
        return movesForPawnAndCard ?? new List<MoveModel>();
    }
}