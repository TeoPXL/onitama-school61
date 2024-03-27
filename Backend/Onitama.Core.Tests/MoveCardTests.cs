using System.Drawing;
using System.Reflection;
using Guts.Client.Core;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.Util.Contracts;
using Onitama.Core.Util;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "MoveCard",
    @"Onitama.Core\MoveCardAggregate\MoveCard.cs;Onitama.Core\MoveCardAggregate\MoveCardFactory.cs;")]
public class MoveCardTests
{
    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(MoveCard).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_IMoveCard()
    {
        Assert.That(typeof(MoveCard).IsAssignableTo(typeof(IMoveCard)), Is.True);
    }

    [MonitoredTest]
    [TestCase("0,0,0,0,1\n" +
              "0,1,0,0,0\n" +
              "0,0,2,0,0\n" +
              "0,0,0,1,0\n" +
              "1,0,0,0,0", 
        "(2,2)", "North", 
        "(0,0);(1,3);(3,1);(4,4)")]
    [TestCase("0,0,0,1,0\n" +
              "0,0,1,0,0\n" +
              "0,0,2,0,0\n" +
              "0,0,0,1,0\n" +
              "1,0,0,0,0",
        "(2,2)", "South",
        "(4,4);(3,1);(1,2);(0,1)")]
    [TestCase("0,0,0,0,1\n" +
              "0,1,0,0,0\n" +
              "0,0,2,0,0\n" +
              "0,0,0,1,0\n" +
              "1,0,0,0,0",
        "(3,1)", "North",
        "(2,2);(4,0)")]
    [TestCase("0,0,0,1,0\n" +
              "0,0,1,0,0\n" +
              "0,0,2,0,0\n" +
              "0,0,0,1,0\n" +
              "1,0,0,0,0",
        "(4,1)", "South",
        "(3,1);(2,0)")]
    public void GetPossibleTargetCoordinates_ShouldReturnCorrectCoordinates(string gridAsText, string startCoordinateAsText, string playDirectionAsText, string expectedCoordinatesAsText)
    {
        // Arrange
        MoveCardGridCellType[,] grid = ConvertGridTextToMoveCardGridCellType(gridAsText);

        IMoveCard moveCard = CreateMoveCard("TestCard", grid, Color.Red);
        ICoordinate startCoordinate = ConvertTextToCoordinate(startCoordinateAsText);
        Direction playDirection = playDirectionAsText;
        int matSize = 5;
        IReadOnlyList<ICoordinate> expectedCoordinates = ConvertTextToCoordinates(expectedCoordinatesAsText);

        // Act
        IReadOnlyList<ICoordinate>? results = moveCard.GetPossibleTargetCoordinates(startCoordinate, playDirection, matSize);

        // Assert
        Assert.That(results,Is.Not.Null, "The list returned is null");
        Assert.That(results.Count, Is.EqualTo(expectedCoordinates.Count), $"An incorrect amount of coordinates is returned");
        foreach (ICoordinate expectedCoordinate in expectedCoordinates)
        {
            Assert.That(results, Contains.Item(expectedCoordinate), $"The coordinate {expectedCoordinate} was expected to be returned, but wasn't");
        }
    }

    private IMoveCard CreateMoveCard(string name, MoveCardGridCellType[,] grid, Color stampColor)
    {
        IMoveCard? card = new MoveCard(name, grid, stampColor) as IMoveCard;
        Assert.That(card, Is.Not.Null, "MoveCard should implement IMoveCard");
        return card!;
    }

    private MoveCardGridCellType[,] ConvertGridTextToMoveCardGridCellType(string gridText)
    {
        string[] lines = gridText.Split('\n');
        var grid = new MoveCardGridCellType[lines.Length, lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(',');

            for (int j = 0; j < cells.Length; j++)
            {
                grid[lines.Length - i - 1, j] = (MoveCardGridCellType)int.Parse(cells[j]);
            }
        }

        return grid;
    }

    private IReadOnlyList<ICoordinate> ConvertTextToCoordinates(string coordinatesText)
    {
        var coordinateStrings = coordinatesText.Split(';');
        var coordinates = new List<ICoordinate>();

        foreach (var coordinateString in coordinateStrings)
        {
            coordinates.Add(ConvertTextToCoordinate(coordinateString));
        }
        return coordinates;
    }

    private ICoordinate ConvertTextToCoordinate(string coordinateText)
    {
        var parts = coordinateText.Trim('(', ')').Split(',');
        return (new Coordinate(int.Parse(parts[0]), int.Parse(parts[1])) as ICoordinate)!;
    }
}