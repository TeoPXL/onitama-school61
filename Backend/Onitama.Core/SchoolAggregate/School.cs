using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using System.Drawing;
using System.Numerics;

namespace Onitama.Core.SchoolAggregate;

/// <inheritdoc cref="ISchool"/>
internal class School : ISchool
{
    private IPawn _master;

    public ICoordinate _archPos;

    public IPawn Master 
    {
        get { return _master; }
        set {
            _master = value;
            _archPos = _master.Position;
        }
    }

    public IPawn[] Students { get; }

    public IPawn[] AllPawns { get; }
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

    public School(IPawn[] pawns)
    {
        //4 for now
        this.AllPawns = new Pawn[5];
        this.AllPawns[0] = pawns[0];
        this.AllPawns[1] = pawns[1];
        this.AllPawns[2] = pawns[2];
        this.AllPawns[3] = pawns[3];
        this.AllPawns[4] = pawns[4];

        this.Students = new Pawn[4];

        Students[0] = AllPawns[0];
        Students[1] = AllPawns[1];
        Students[2] = AllPawns[2];
        Students[3] = AllPawns[3];

        SetMaster(AllPawns[2]);
    }

    public ICoordinate TempleArchPosition 
    {
        get { return _archPos; }
        set { _archPos = value; }
    }

    public IPawn GetPawn(Guid pawnId)
    {
        return this.AllPawns.FirstOrDefault(p => p.Id == pawnId);
    }

    public void SetMaster(IPawn master)
    {
        this._master = master;
    }

    public void SetStudent(IPawn student, int i)
    {
        this.Students[i] = student;
    }
}