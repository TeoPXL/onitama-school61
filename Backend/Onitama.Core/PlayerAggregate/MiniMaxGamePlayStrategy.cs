using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;

namespace Onitama.Core.PlayerAggregate;

/// <summary>
/// <see cref="IGamePlayStrategy"/> that uses the mini-max algorithm (https://en.wikipedia.org/wiki/Minimax).
/// </summary>
/// <remarks>
/// This is an EXTRA. Not needed to implement the minimal requirements.
/// Also when implementing the (AI) extra, it is not needed to alter code in this class.
/// It should work as is.
/// </remarks>
internal class MiniMaxGamePlayStrategy : IGamePlayStrategy
{
    private Guid _maximizingPlayerId;
    private Guid _minimizingPlayerId;

    private readonly int _maximumDepth;
    private readonly IGameEvaluator _gameEvaluator;


    public MiniMaxGamePlayStrategy(IGameEvaluator gameEvaluator, int maximumDepth)
    {
        _maximumDepth = maximumDepth;
        _gameEvaluator = gameEvaluator;
    }


    public IMove GetBestMoveFor(Guid playerId, IGame game)
    {
        var scoreDictionary = new Dictionary<IMove, int>();

        _maximizingPlayerId = playerId;
        _minimizingPlayerId = game.Players.First(p => p.Id != playerId).Id;

        //Get all possible moves
        IReadOnlyList<IMove> possibleMoves = GetPossibleMoves(playerId, game);

        //Link the lowest possible initial score to each move
        foreach (IMove move in possibleMoves)
        {
            scoreDictionary.Add(move, int.MinValue);
        }

        //Apply the mini-max algorithm for each possible move
        foreach (IMove move in possibleMoves)
        {
            IGame newGame = new Game(game) as IGame;
            newGame.MovePawn(playerId, move.Pawn.Id, move.Card.Name, move.To);
            scoreDictionary[move] = MiniMax(newGame, _minimizingPlayerId, _maximumDepth, int.MinValue, int.MaxValue);
        }

        //Get the moves with the highest score (multiple moves may have the same score)
        int bestScore = scoreDictionary.Max(kv => kv.Value);
        IMove[] bestMoves = scoreDictionary.Where(kv => kv.Value == bestScore).Select(kv => kv.Key).ToArray();

        if (bestMoves.Length == 0) return null;

        //Choose a move from the best moves
        IMove chosenMove = bestMoves[Random.Shared.Next(bestMoves.Length)];
        return chosenMove;
    }

    private IReadOnlyList<IMove> GetPossibleMoves(Guid playerId, IGame game)
    {
        IReadOnlyList<IMove> possibleMoves = game.GetAllPossibleMovesFor(playerId);
        if (possibleMoves.Count == 0)
        {
            var cardExchangeMoves = new List<IMove>();
            IPlayer player = game.Players.First(p => p.Id == playerId);
            foreach (IMoveCard card in player.MoveCards)
            {
                cardExchangeMoves.Add(new Move(card));
            }
            possibleMoves = cardExchangeMoves;
        }

        return possibleMoves;
    }

    private int MiniMax(IGame game, Guid playerId, int depth, int alpha, int beta)
    {
        if (depth == 0 || game.WinnerPlayerId != Guid.Empty)
        {
            //Reached a leaf node in the recursion or the game has a winning connection -> do not recurse further. Return the score of the grid.
            return _gameEvaluator.CalculateScore(game, _maximizingPlayerId);
        }

        bool isMaximizingPlayer = playerId == _maximizingPlayerId;
        Guid opponentId = isMaximizingPlayer ? _minimizingPlayerId : _maximizingPlayerId;
        int bestScore = isMaximizingPlayer ? int.MinValue : int.MaxValue;
        int worstScore = isMaximizingPlayer ? int.MaxValue : int.MinValue;

        //Get the moves the player can execute
        IReadOnlyList<IMove> possibleMoves = GetPossibleMoves(playerId, game);

        int moveCount = 0;
        while (moveCount < possibleMoves.Count && alpha < beta)  //alpha < beta = Alpha-Beta pruning -> https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning
        {
            //create a copy of the game and apply a move on it
            IMove move = possibleMoves[moveCount];
            IGame newGame = new Game(game) as IGame;

            if (move.Pawn is null)
            {
                newGame.SkipMovementAndExchangeCard(playerId, move.Card.Name);
            }
            else
            {
                newGame.MovePawn(playerId, move.Pawn.Id, move.Card.Name, move.To);
            }
           
            moveCount++;

            //Recurse (go one level deeper) - Apply MiniMax from opponent perspective
            int score = MiniMax(newGame, opponentId, depth - 1, alpha, beta);

            //Remember the best score (positive for maximizing player, negative for minimizing player)
            if (isMaximizingPlayer)
            {
                if (score > bestScore)
                {
                    bestScore = score;
                    alpha = Math.Max(alpha, bestScore);
                }
                
            }
            else
            {
                if (score < bestScore)
                {
                    bestScore = score;
                    beta = Math.Min(beta, bestScore);
                }
            }
        }

        return bestScore;
    }
}