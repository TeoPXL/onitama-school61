using Onitama.Core.UserAggregate;

namespace Onitama.Core.TableAggregate.Contracts;

/// <summary>
/// Used to create new tables.
/// </summary>
public interface ITableFactory
{
    /// <summary>
    /// Creates a new table for a user with the given preferences.
    /// The user is automatically joined to the table and becomes the owner of the table.
    /// </summary>
    /// <param name="user">The user</param>
    /// <param name="preferences">Determine the game options (e.g. play mat size, number of players, ...)</param>
    ITable CreateNewForUser(User user, TablePreferences preferences);
}