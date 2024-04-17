using System.Drawing;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.Tests.Extensions;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Tests.Builders;

public class PlayerMockBuilder : MockBuilder<IPlayer>
{
    private static readonly Color[] AllColors = new[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange };
    private int _colorIndex;

    public PlayerMockBuilder()
    {
        _colorIndex = Random.Shared.Next(AllColors.Length);
        Guid id = Guid.NewGuid();
        Mock.SetupGet(p => p.Id).Returns(id);
        Mock.SetupGet(p => p.Color).Returns(AllColors[_colorIndex]);
        Mock.SetupGet(p => p.Direction).Returns(Direction.North);
        Mock.SetupGet(p => p.Name).Returns("Player");
        Mock.SetupGet(p => p.School).Returns(new SchoolMockBuilder(Direction.North).WithOwner(id).Object);
        Mock.SetupGet(p => p.MoveCards).Returns(new List<IMoveCard>());

        _colorIndex = (_colorIndex + 1) % AllColors.Length;
    }

    public PlayerMockBuilder BasedOnUser(User user)
    {
        Mock.SetupGet(p => p.Id).Returns(user.Id);
        Mock.SetupGet(p => p.Name).Returns(user.WarriorName);
        Mock.SetupGet(p => p.Color).Returns(AllColors[_colorIndex]);
        Mock.SetupGet(p => p.School).Returns(new SchoolMockBuilder(Direction.North).WithOwner(user.Id).Object);
        _colorIndex = (_colorIndex + 1) % AllColors.Length;
        return this;
    }

    public PlayerMockBuilder WithMoveCards()
    {
        return WithMoveCards(new MoveCardMockBuilder().Object, new MoveCardMockBuilder().Object);
    }

    public PlayerMockBuilder WithMoveCards(params IMoveCard[] moveCards)
    {
        Mock.SetupGet(p => p.MoveCards).Returns(moveCards.ToList());
        return this;
    }

    public PlayerMockBuilder WithDirection(Direction direction)
    {
        Guid playerId = Mock.Object.Id;
        Mock.SetupGet(p => p.Direction).Returns(direction);
        Mock.SetupGet(p => p.School).Returns(new SchoolMockBuilder(direction).WithOwner(playerId).Object);
        return this;
    }

    public PlayerMockBuilder WithColor(Color color)
    {
        Mock.SetupGet(p => p.Color).Returns(color);
        return this;
    }
}