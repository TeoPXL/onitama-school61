using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Numerics;

namespace Onitama.Core.SchoolAggregate;

/// <inheritdoc cref="ISchool"/>
internal class School : ISchool
{
    private IPawn _master;

    public ICoordinate _archPos;

    public IPawn[] _allPawns; 

    public IPawn[] _students;

    public IPawn Master 
    {
        get { return _master; }
        set {
            _master = value;
            _archPos = _master.Position;
        }
    }

    public IPawn[] Students {
        get { return _students; }
    }

    public IPawn[] AllPawns {
        get { return _allPawns; }
    }
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
        this._allPawns = new Pawn[5];
        this._allPawns[0] = pawns[0];
        this._allPawns[1] = pawns[1];
        this._allPawns[2] = pawns[2];
        this._allPawns[3] = pawns[3];
        this._allPawns[4] = pawns[4];

        this._students = new Pawn[4];

        _students[0] = _allPawns[0];
        _students[1] = _allPawns[1];
        _students[2] = _allPawns[2];
        _students[3] = _allPawns[3];

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

    public void RemovePawn(IPawn pawnToRemove)
    {
        int index = Array.IndexOf(AllPawns, pawnToRemove);
        if (index != -1)
        {
            AllPawns[index].Position = null;
        }

        index = Array.IndexOf(Students, pawnToRemove);
        if (index != -1)
        {
            Students[index].Position = null;
        }
    }
}