using Microsoft.AspNetCore.Identity;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;

namespace Onitama.Core.GameAggregate.Contracts;

public interface IGameFactory
{

    IGame CreateNewForTable(ITable table);
}