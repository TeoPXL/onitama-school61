﻿using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using System.Drawing;

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
        get { return _archPos; }
        set { _archPos = value; }
    }

    public IPawn GetPawn(Guid pawnId)
    {
        return this.AllPawns.FirstOrDefault(p => p.Id == pawnId);
    }
}