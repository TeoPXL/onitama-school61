namespace Onitama.Core.GameAggregate.Contracts;

/// <summary>
/// Can give a score on a game. The score indicates who is winning.
/// </summary>
/// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
public interface IGameEvaluator
{
    /// <summary>
    /// Calculates a score for a <see cref="IGame"/>.
    /// The score indicates how likely is is that the <paramref name="maximizingPlayerId"/> is going to win.
    /// If the <paramref name="maximizingPlayerId"/> is winning the score will be positive.
    /// If the <paramref name="maximizingPlayerId"/> is losing the score will be negative.
    /// </summary>
    /// <param name="game">The game</param>
    /// <param name="maximizingPlayerId">
    /// The identifier of the player that tries to get a score as high as possible.
    /// The minimizing player (the other player) is winning when the score is negative.
    /// </param>
    /// <returns>
    /// 0 if nobody has the upper hand.
    /// A positive score if the maximizing player has the upper hand. The higher the score, the more likely the maximizing player is going to win.
    /// A negative score if the minimizing player has the upper hand. The lower the score, the more likely the minimizing player is going to win.
    /// int.MinValue when the minimizing player has won.
    /// int.MaxValue when the maximizing player has won.
    /// </returns>
    int CalculateScore(IGame game, Guid maximizingPlayerId);
}