using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using Onitama.Core.MoveCardAggregate.Contracts;

namespace Onitama.Infrastructure;

internal class MoveCardFileRepository : IMoveCardRepository
{
    private readonly IMoveCardFactory _moveCardFactory;

    public MoveCardFileRepository(IMoveCardFactory moveCardFactory)
    {
        _moveCardFactory = moveCardFactory;
    }

    public IMoveCard[] LoadSet(MoveCardSet set, Color[] possibleStampColors)
    {
        string fileName;
        switch (set)
        {
            case MoveCardSet.Original:
                fileName = "original.json";
                break;
            case MoveCardSet.SenseisPath:
                fileName = "sensei-s-path.json";
                break;
            case MoveCardSet.WayOfTheWind:
                fileName = "way-of-the-wind.json";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(set), set, "Error loading movecard set");
        }

        string json = File.ReadAllText($"CardSets\\{fileName}");
        List<FileMoveCard> fileMoveCards = JsonSerializer.Deserialize<List<FileMoveCard>>(json);
        var moveCards = new List<IMoveCard>();
        foreach (FileMoveCard fileMoveCard in fileMoveCards)
        {
            var grid = new MoveCardGridCellType[5, 5];

            // Reverse the rows from the card in the file, because the file card is written top to bottom, but the grid is bottom to top
            IList<string> fileRows = fileMoveCard.Grid.Reverse().ToList();

            for (int row = 0; row < 5; row++)
            {
                for (int column = 0; column < 5; column++)
                {
                    MoveCardGridCellType value = Enum.Parse<MoveCardGridCellType>(fileRows[row].Substring(column, 1));
                    grid[row, column] = value;
                }
            }
            var moveCard = _moveCardFactory.Create(fileMoveCard.Name, grid, possibleStampColors);
            moveCards.Add(moveCard);
        }
        return moveCards.ToArray();
    }

    public IMoveCard[] LoadSetCustom(MoveCardSet set, Color[] possibleStampColors, string customJson = null)
    {
        string json;

        switch (set)
        {
            case MoveCardSet.Original:
                json = File.ReadAllText("CardSets\\original.json");
                break;
            case MoveCardSet.SenseisPath:
                json = File.ReadAllText("CardSets\\sensei-s-path.json");
                break;
            case MoveCardSet.WayOfTheWind:
                json = File.ReadAllText("CardSets\\way-of-the-wind.json");
                break;
            case MoveCardSet.Custom:
                if (string.IsNullOrWhiteSpace(customJson))
                    throw new ArgumentException("Custom JSON string cannot be null or empty for custom set.", nameof(customJson));
                json = customJson;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(set), set, "Error loading movecard set");
        }

        List<FileMoveCard> fileMoveCards = JsonSerializer.Deserialize<List<FileMoveCard>>(json);
        var moveCards = new List<IMoveCard>();

        foreach (FileMoveCard fileMoveCard in fileMoveCards)
        {
            var grid = new MoveCardGridCellType[5, 5];
            var altGrid = new MoveCardGridCellType[5, 5];

            // Reverse the rows from the card in the file, because the file card is written top to bottom, but the grid is bottom to top
            IList<string> fileRows = fileMoveCard.Grid.Reverse().ToList();
            IList<string> altRows = [];
            if (set == MoveCardSet.WayOfTheWind)
            {
                altRows = fileMoveCard.AltGrid.Reverse().ToList();
                for (int row = 0; row < 5; row++)
                {
                    for (int column = 0; column < 5; column++)
                    {
                        MoveCardGridCellType value = Enum.Parse<MoveCardGridCellType>(fileRows[row].Substring(column, 1));
                        grid[row, column] = value;
                        MoveCardGridCellType val = Enum.Parse<MoveCardGridCellType>(altRows[row].Substring(column, 1));
                        altGrid[row, column] = val;
                    }
                }
            } else
            {
                for (int row = 0; row < 5; row++)
                {
                    for (int column = 0; column < 5; column++)
                    {
                        MoveCardGridCellType value = Enum.Parse<MoveCardGridCellType>(fileRows[row].Substring(column, 1));
                        grid[row, column] = value;
                    }
                }
            }

            var moveCard = _moveCardFactory.Create(fileMoveCard.Name, grid, possibleStampColors);

            if (set == MoveCardSet.WayOfTheWind)
            {
                moveCard = _moveCardFactory.CreateAlt(fileMoveCard.Name, grid, altGrid, possibleStampColors);
            }
            moveCards.Add(moveCard);
        }

        return moveCards.ToArray();
    }


    private class FileMoveCard
    {
        public string Name { get; set; }
        public string[] Grid { get; set; }
        public string[] AltGrid { get; set; }
    }
}