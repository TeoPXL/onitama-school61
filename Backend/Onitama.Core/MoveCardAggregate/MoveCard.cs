using System.Drawing;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.MoveCardAggregate;

/// <inheritdoc cref="IMoveCard"/>
internal class MoveCard : IMoveCard
{
    private Color _stampColor;
    private MoveCardGridCellType[,] _grid;
    private MoveCardGridCellType[,] _gridSouth;
    private MoveCardGridCellType[,] _gridWest;
    private MoveCardGridCellType[,] _gridEast;
    public string Name { get; }

    public MoveCardGridCellType[,] Grid
    {
        get { return _grid; }
        set { this._grid = value; }
    }

    public Color StampColor
    {
        get { return _stampColor; }
        set { this._stampColor = value; }
    }

    public MoveCard(string name, MoveCardGridCellType[,] grid, Color stampColor)
    {
        this._grid = grid;
        this._gridSouth = RotateGrid(2);
        this._gridWest = RotateGrid(3);
        this._gridEast = RotateGrid(1);
        this._stampColor = stampColor;
        this.Name = name;
    }

    //Do not change this method, it makes sure that two MoveCard instances are equal if their names are equal
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        return obj is IMoveCard other && Equals(other);
    }

    //Do not change this method
    protected bool Equals(IMoveCard other)
    {
        return Name == other.Name;
    }

    //Do not change this method
    public override int GetHashCode()
    {
        return (Name != null ? Name.GetHashCode() : 0);
    }

    private MoveCardGridCellType[,] RotateGrid(int number)
    {
        var grid = _grid;
        for (int i = 0; i < number; i++)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            // Create a new transposed array
            MoveCardGridCellType[,] rotatedGrid = new MoveCardGridCellType[cols, rows];

            // Perform transpose
            for (int k = 0; k < rows; k++)
            {
                for (int j = 0; j < cols; j++)
                {
                    rotatedGrid[j, k] = grid[k, j];
                }
            }

            // Reverse each row in the transposed array
            for (int k = 0; k < cols; k++)
            {
                for (int j = 0; j < rows / 2; j++)
                {
                    MoveCardGridCellType temp = rotatedGrid[k, j];
                    rotatedGrid[k, j] = rotatedGrid[k, rows - 1 - j];
                    rotatedGrid[k, rows - 1 - j] = temp;
                }
            }
            grid = rotatedGrid;

        }
        return grid;
    }

    public IReadOnlyList<ICoordinate> GetPossibleTargetCoordinates(ICoordinate startCoordinate, Direction playDirection, int matSize)
    {
        var grid = _grid;
        switch (playDirection)
        {
            case var d when d == Direction.South:
                grid = _gridSouth; break;
            case var d when d == Direction.West:
                grid = _gridWest; break;
            case var d when d == Direction.East:
                grid = _gridEast; break;
            default:
                break;
        }
        var list = new List<ICoordinate>();
        for (int i = 0; i < Math.Sqrt(grid.Length); i++)
        {
            for (int k = 0; k < Math.Sqrt(grid.Length); k++)
            {
                var coord = grid[i, k];
                if (coord == MoveCardGridCellType.Target)
                {
                    var endX = startCoordinate.Column - 2 + k;
                    var endY = startCoordinate.Row - 2 + i;
                    if(endX < 5 && endY < 5 && endX >= 0 && endY >= 0)
                    {
                        list.Add(new Coordinate(endY, endX));
                    }
                }
            }
        }
        return list;
    }
}