using Onitama.Core;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Infrastructure.Util;

namespace Onitama.Infrastructure;

/// <inheritdoc cref="ITableRepository"/>
internal class InMemoryTableRepository : ITableRepository
{
    private readonly ExpiringDictionary<Guid, ITable> _tableDictionary;

    public InMemoryTableRepository()
    {
        _tableDictionary = new ExpiringDictionary<Guid, ITable>(TimeSpan.FromMinutes(15));
    }

    public void Add(ITable table)
    {
        _tableDictionary.AddOrReplace(table.Id, table);
    }

    public ITable Get(Guid tableId)
    {
        if (_tableDictionary.TryGetValue(tableId, out ITable table))
        {
            return table!;
        }
        throw new DataNotFoundException();
    }

    public void Remove(Guid tableId)
    {
        _tableDictionary.TryRemove(tableId, out ITable _);
    }

    public IList<ITable> FindTablesWithAvailableSeats()
    {
        //TODO: loop over all tables (user the Values property of _tableDictionary)
        //and check if those tables have seats available.
        //Put the tables that have available seats in a list and return that list.
       
        throw new NotImplementedException();
    }
}