using System.Drawing;
using System.Text.Json;
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

    private class FileMoveCard
    {
        public string Name { get; set; }
        public string[] Grid { get; set; }
    }
}