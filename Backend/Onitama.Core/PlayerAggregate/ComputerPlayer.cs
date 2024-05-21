using System.Drawing;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;

namespace Onitama.Core.PlayerAggregate;

/// <inheritdoc cref="IPlayer"/>
internal class ComputerPlayer : PlayerBase
{
    public IGamePlayStrategy Strategy { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Time { get; set; }

    public Color Color { get; set; }

    public Direction Direction { get; set; }

    public ISchool School { get; set; }

    public int Elo { get; set; }
    public User User { get; set; }
    public bool HasValidMoves { get; set; }

    private IList<IMoveCard> _moveCards;
    public IList<IMoveCard> MoveCards
    {
        get { return _moveCards; }
        set { this._moveCards = value; }
    }

    public ComputerPlayer(Color color, Direction direction, IGamePlayStrategy strategy) : base(Guid.Parse("10000000000000000000000000000001"), "OnitamaBot", color, direction)
    {
        Strategy = strategy;
        Color = color;
        Direction = direction;
        Elo = 9999;
        User = null;
        HasValidMoves = false;
        Name = "OnitamaBot";
        Id = Guid.Parse("10000000000000000000000000000001");
    }


    /// <summary>
    /// Uses gameplay strategy to determine the best move to execute.
    /// </summary>
    /// <param name="game">The game (in its current state)</param>
    public override IMove DetermineBestMove(IGame game)
    {
        return Strategy.GetBestMoveFor(Id, new Game(game));
    }

    public void SetSchool(ISchool school)
    {
        School = school;
    }
}