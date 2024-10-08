﻿using System.Drawing;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.UserAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.GameAggregate;
using System.Numerics;

namespace Onitama.Core.PlayerAggregate;

/// <inheritdoc cref="IPlayer"/>
internal class PlayerBase : IPlayer
{
    private ISchool _school;
    public Guid Id { get; }
    public string Name { get; }
    public Color Color { get; }
    public Direction Direction { get; }

    public IGamePlayStrategy Strategy { get; set; }


    private int _elo;
    private User _user;
    private bool _hasValidMoves = false;
    public bool HasValidMoves
    {
        get { return _hasValidMoves; }
        set { this._hasValidMoves = value; }
    }

    public virtual int Elo {
        get { return _elo; }
        set { _elo = value; }
    }

    public virtual User User
    {
        get { return _user; }
        set { _user = value; }
    }

    private int _time = 180;
    public int Time {
        get { return _time; }
        set { _time = value; }
    }
    public ISchool School
    {
        get { return _school; }
    }
    public IList<IMoveCard> MoveCards { get; set; } 

    protected PlayerBase(Guid id, string name, Color color, Direction direction)
    {
        Id = id;
        Name = name;
        Color = color;
        Direction = direction;
        MoveCards = new List<IMoveCard>(); 
    }

    /// <summary>
    /// Creates a player that is a copy of an other player.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public PlayerBase(IPlayer otherPlayer)
    {
        SetSchool(otherPlayer.School);
        Id = otherPlayer.Id;
        Name = otherPlayer.Name;
        Color = otherPlayer.Color;
        Direction = otherPlayer.Direction;
        MoveCards = [.. otherPlayer.MoveCards];
        Strategy = otherPlayer.Strategy;
    }

    public void SetSchool(ISchool school)
    {
        var pawns = new IPawn[5];
        for (int i = 0; i < school.AllPawns.Length; i++)
        {
            var pawn = school.AllPawns[i];
            var newPawn = new Pawn(pawn.Id, pawn.OwnerId, pawn.Type);
            newPawn.Position = pawn.Position;
            pawns[i] = newPawn;
        }
        this._school = new School(pawns);
        this._school.TempleArchPosition = school.TempleArchPosition;
    }

    public virtual IMove DetermineBestMove(IGame game)
    {
        if(Strategy == null)
        {
            return new Move(MoveCards[0]);
        }
        return Strategy.GetBestMoveFor(Id, new Game(game));
    }
}