using Microsoft.AspNetCore.Identity;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.UserAggregate;

namespace Onitama.Core.TableAggregate.Contracts
{
    /// <summary>
    /// Manages all the tables of the application
    /// </summary>
    public interface ITableManager
    {

        /// <summary>
        /// Creates a new table for a user with the given preferences.
        /// </summary>
        ITable AddNewTableForUser(User user, TablePreferences preferences);

        /// <summary>
        /// Creates a competitive table for a user with the given preferences.
        /// </summary>
        ITable AddCompTableForUser(User user, CompTablePreferences preferences);

        /// <summary>
        /// Creates a blitz table for a user with the given preferences.
        /// </summary>
        ITable AddBlitzTableForUser(User user, BlitzTablePreferences preferences);

        /// <summary>
        /// Creates a custom table for a user with the given preferences.
        /// </summary>
        ITable AddCustomTableForUser(User user, CustomTablePreferences preferences);

        /// <summary>
        /// Joins a user to a table.
        /// </summary>
        void JoinTable(Guid tableId, User user);

        /// <summary>
        /// Removes a user from a table.
        /// </summary>
        void LeaveTable(Guid tableId, User user);

        /// <summary>
        /// EXTRA: Fills the table with computer players.
        /// </summary>
        /// <remarks>This is an EXTRA. Not needed to implement the minimal requirements.</remarks>
        void FillWithArtificialPlayers(Guid tableId, User user);

        /// <summary>
        /// Starts a game for a table.
        /// </summary>
        IGame StartGameForTable(Guid tableId, User user);
        IGame StartGameForTableAi(Guid tableId, User user);
    }
}
