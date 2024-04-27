using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using System.Drawing;

namespace Onitama.Core.SchoolAggregate;

/// <inheritdoc cref="ISchool"/>
internal class School : ISchool
{

    public IPawn Master { get; set; }

    public IPawn[] Students { get; set; }

    public IPawn[] AllPawns { get; set; }
    /// <summary>
    /// Creates a school that is a copy of another school.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public School(ISchool otherSchool)
    {
        throw new NotImplementedException("TODO: copy properties of other school. Make sure to copy the pawns, not just reference them");
    }

    public School()
    {

    }

    public ICoordinate TempleArchPosition 
    {
        get { return this.Master.Position; }
        set { this.Master.Position = value; }
    }

    public IPawn GetPawn(Guid pawnId)
    {
        return this.AllPawns.FirstOrDefault(p => p.Id == pawnId);
    }
}