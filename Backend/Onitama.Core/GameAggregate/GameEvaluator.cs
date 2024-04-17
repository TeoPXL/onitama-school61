using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;

namespace Onitama.Core.GameAggregate;

/// <inheritdoc cref="IGameEvaluator"/>
internal class GameEvaluator : IGameEvaluator
{
    public int CalculateScore(IGame game, Guid maximizingPlayerId)
    {
        throw new NotImplementedException();
    }
}