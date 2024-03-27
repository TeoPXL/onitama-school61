using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.TableAggregate;

namespace Onitama.Core.Tests.Builders;

public class TablePreferencesBuilder
{
    private readonly TablePreferences _preferences;

    public TablePreferencesBuilder()
    {
        _preferences = new TablePreferences
        {
            PlayerMatSize = 5,
            NumberOfPlayers = 2,
            MoveCardSet = MoveCardSet.Original
        };
    }

    public TablePreferences Build()
    {
        return _preferences;
    }
}
