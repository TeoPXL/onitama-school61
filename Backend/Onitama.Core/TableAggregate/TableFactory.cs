using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;

namespace Onitama.Core.TableAggregate;

/// <inheritdoc cref="ITableFactory"/>
internal class TableFactory : ITableFactory
{
    public ITable CreateNewForUser(User user, TablePreferences preferences)
    {
        var table = new Table(Guid.NewGuid(), preferences);
        table.Join(user);
        return table;
    }

    public ITable CreateCompForUser(User user, CompTablePreferences preferences)
    {
        var table = new Table(Guid.NewGuid(), preferences);
        table.Join(user);
        return table;
    }

    public ITable CreateBlitzForUser(User user, BlitzTablePreferences preferences)
    {
        var table = new Table(Guid.NewGuid(), preferences);
        table.Join(user);
        return table;
    }

    public ITable CreateCustomForUser(User user, CustomTablePreferences preferences)
    {
        var table = new Table(Guid.NewGuid(), preferences);
        table.Join(user);
        return table;
    }
}