using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.UserAggregate;
using System.ComponentModel;

namespace Onitama.Core.TableAggregate.Contracts
{
    public interface ITablePreferences
    {
        int NumberOfPlayers { get; set; }

        int PlayerMatSize { get; set; }

        MoveCardSet MoveCardSet { get; set; }

        string TableType { get; set; }

        string MoveCardString { get; set; }
    }
}
