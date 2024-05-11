using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using System.Collections.Generic;

namespace Onitama.Core.PlayMatAggregate
{
    /// <summary>
    /// Represents the play mat of the game.
    /// </summary>
    internal class PlayMat : IPlayMat
    {
        private IPawn[,] _grid;
        private int _size;

        public IPawn[,] Grid
        {
            get { return _grid; }
        }

        public int Size
        {
            get { return _size; }
        }

        public PlayMat(int size)
        {
            _size = size;
            _grid = new IPawn[size, size];
        }

        public PlayMat(IPlayMat otherPlayMat, IPlayer[] copiedPlayers)
        {
            //"TODO: copy properties of other playmat"
            
            _size = otherPlayMat.Size;
            _grid = new IPawn[_size, _size];

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    _grid[i, j] = otherPlayMat.Grid[i, j];
                }
            }
        }

        public void PositionSchoolOfPlayer(IPlayer player)
        {
            //This code is very sketchy. I honestly do not really understand why this works? I just threw stuff at the wall to see what stuck.
            
            var pawns = player.School.AllPawns;
            for (int i = 0; i < pawns.Length; i++)
            {
                Coordinate coordinate;
                switch (player.Direction)
                {
                    case var d when d == Direction.North:
                        coordinate = new Coordinate(0, i);
                        break;
                    case var d when d == Direction.South:
                        coordinate = new Coordinate(this.Size - 1, i);
                        break;
                    case var d when d == Direction.West:
                        coordinate = new Coordinate(i, this.Size - 1);
                        break;
                    case var d when d == Direction.East:
                        coordinate = new Coordinate(i, 0);
                        break;
                    default:
                        coordinate = new Coordinate(0, 0);
                        break;
                }

                pawns[i].Position = coordinate;
                _grid[pawns[i].Position.Row, pawns[i].Position.Column] = pawns[i];
                if (pawns[i].Type == PawnType.Master)
                {
                    Coordinate copiedCoordinate = new Coordinate(coordinate.Row, coordinate.Column);
                    player.School.TempleArchPosition = copiedCoordinate;
                }
            }
        }

        public IReadOnlyList<IMove> GetValidMoves(IPawn pawn, IMoveCard card, Direction playerDirection)
        {
            List<IMove> moves = new List<IMove>();
            var possibleMoves = card.GetPossibleTargetCoordinates(pawn.Position, playerDirection, _size);
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                var x = possibleMoves[i].Column;
                var y = possibleMoves[i].Row;
                bool isAvailable = true;

                for (int j = 0; j < _grid.GetLength(0); j++)
                {
                    for (int k = 0; k < _grid.GetLength(1); k++)
                    {
                        var item = _grid[j, k];
                        if(item is IPawn)
                        {
                            if (item.Position.Row == y && item.Position.Column == x && item.OwnerId == pawn.OwnerId)
                            {
                                isAvailable = false;
                            }
                        }
                        
                    }
                }

                if(isAvailable == true)
                {
                    var move = new Move(card, pawn, playerDirection, possibleMoves[i]);
                    moves.Add(move);
                }
                
            }
            return moves;
        }
         
        public void ExecuteMove(IMove move, out IPawn capturedPawn)
        {
            capturedPawn = null;
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i, j] != null && _grid[i, j].Position.Row == move.To.Row && _grid[i, j].Position.Column == move.To.Column)
                    {
                        capturedPawn = _grid[i, j];
                        break;
                    }
                }
            }
            move.Pawn.Position = move.To;
        }

        public void RemovePawn(IPawn pawn)
        {
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i, j] != null && _grid[i, j].Id == pawn.Id)
                    {
                        _grid[i, j] = null;
                        return;
                    }
                }
            }
        }
    }
}
