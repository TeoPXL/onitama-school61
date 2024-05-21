using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using System;

namespace Onitama.Core.GameAggregate;

/// <inheritdoc cref="IGameEvaluator"/>
internal class GameEvaluator : IGameEvaluator
{
    public int CalculateScore(IGame game, Guid maximizingPlayerId) //I kinda wanna add an "aggressiveness" factor at some point, if we have time
    {
        int score = 0; //Start with a neutral score
        IPlayer maximizingPlayer = null;
        IPlayer minimizingPlayer = null;

        foreach(var player in game.Players)
        {
            if(player.Id == maximizingPlayerId)
            {
                maximizingPlayer = player;
            } else
            {
                minimizingPlayer = player;
            }
        }

        if(maximizingPlayer == null) 
        {
            throw new InvalidOperationException("This player does not exist.");
        }

        //Take into account a difference in pawns
        var grid = game.PlayMat.Grid;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != null && grid[i, j].OwnerId == maximizingPlayerId)
                {
                    score += 5;
                }
                if(grid[i, j] != null && grid[i, j].OwnerId != maximizingPlayerId)
                {
                    score -= 5;
                }
            }
        }

        //Take into account the amount of space controlled
        score += game.GetAllPossibleMovesFor(maximizingPlayerId).Count;
        score -= game.GetAllPossibleMovesFor(minimizingPlayer.Id).Count;

        //Take into account control of the center
        if (grid[0, 2] != null && grid[0, 2].Id == maximizingPlayerId)
        {
            score++;
        }
        if (grid[0, 2] != null && grid[0, 2].Id == minimizingPlayer.Id)
        {
            score--;
        }
        if (grid[4, 2] != null && grid[4, 2].Id == maximizingPlayerId)
        {
            score++;
        }
        if (grid[4, 2] != null && grid[4, 2].Id == minimizingPlayer.Id)
        {
            score--;
        }

        //Take into account the winning condition
        if(game.WinnerPlayerId == maximizingPlayerId)
        {
            score += 50;
        }

        if (game.WinnerPlayerId == minimizingPlayer.Id)
        {
            score -= 50;
        }

        return score;
    }
}