using Onitama.Core.TableAggregate.Contracts;

namespace Onitama.Core.GameAggregate.Contracts;

public interface IGameFactory
{
    IGame CreateNewForTable(ITable table);
}