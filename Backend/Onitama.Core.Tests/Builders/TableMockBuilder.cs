using Moq;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.TableAggregate;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;

namespace Onitama.Core.Tests.Builders;

public class TableMockBuilder : MockBuilder<ITable>
{
    private readonly TablePreferences _tablePreferences = new TablePreferencesBuilder().Build();

    public TableMockBuilder()
    {
        Mock.SetupGet(t => t.Id).Returns(Guid.NewGuid());
        Mock.SetupGet(t => t.SeatedPlayers).Returns([]);
        Mock.SetupGet(t => t.HasAvailableSeat).Returns(true);
        Mock.SetupGet(t => t.GameId).Returns(Guid.Empty);
        Mock.SetupGet(t => t.OwnerPlayerId).Returns(Guid.NewGuid());
        Mock.SetupGet(t => t.Preferences).Returns(_tablePreferences);
    }

    public TableMockBuilder WithSeatedUsers(User[] users)
    {
        int directionIndex = 0;
        IPlayer[] players = new IPlayer[users.Length];
        for (int i = 0; i < users.Length; i++)
        {
            IPlayer player = new PlayerMockBuilder()
                .BasedOnUser(users[i])
                .WithDirection(Direction.MainDirections[directionIndex])
                .Object;
            players[i] = player;
            directionIndex = (directionIndex + 1) % Direction.MainDirections.Length;
        }
        Mock.SetupGet(t => t.SeatedPlayers).Returns(() => players);
        Mock.SetupGet(t => t.OwnerPlayerId).Returns(players.FirstOrDefault()?.Id ?? Guid.Empty);
        Mock.Setup(table => table.Leave(It.IsAny<Guid>())).Callback((Guid playerId) =>
        {
            players = players.Where(p => p.Id != playerId).ToArray();
        });
        Mock.SetupGet(t => t.HasAvailableSeat).Returns(() => players.Length < _tablePreferences.NumberOfPlayers);

        return this;
    }
}