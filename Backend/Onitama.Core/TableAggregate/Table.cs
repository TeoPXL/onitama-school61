using System.Drawing;
using System.Numerics;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;

namespace Onitama.Core.TableAggregate;

/// <inheritdoc cref="ITable"/>
internal class Table
{
    private static readonly Color[] PossibleColors =
        new[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange };

    public Table(Guid id, TablePreferences preferences)
    {
        
    }
}