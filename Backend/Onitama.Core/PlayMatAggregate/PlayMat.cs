﻿using Onitama.Core.GameAggregate;
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
            // Implementation goes here
        }

        public IReadOnlyList<IMove> GetValidMoves(IPawn pawn, IMoveCard card, Direction playerDirection)
        {
            // Implementation goes here
            return null; // Placeholder return
        }

        public void ExecuteMove(IMove move, out IPawn capturedPawn)
        {
            // Implementation goes here
            capturedPawn = null; // Placeholder
        }
    }
}
