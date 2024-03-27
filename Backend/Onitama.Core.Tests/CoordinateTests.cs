using Guts.Client.Core;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "Coordinate",
    @"Onitama.Core\Util\Coordinate.cs;Onitama.Core\Util\CoordinateFactory.cs;")]
public class CoordinateTests
{
    [MonitoredTest]
    public void Class_ShouldBeInternal_SoThatItCanOnlyBeUsedInTheCoreProject()
    {
        Assert.That(typeof(Coordinate).IsNotPublic, Is.True, "use 'internal class' instead of 'public class'");
    }

    [MonitoredTest]
    public void Class_ShouldImplement_ICoordinate()
    {
        Assert.That(typeof(Coordinate).IsAssignableTo(typeof(ICoordinate)), Is.True);
    }

    [MonitoredTest]
    [TestCase("North", 0, 0, 0, 0)]
    [TestCase("North", 1, 2, 1, 2)]
    [TestCase("South", 1, 2, -1, -2)]
    [TestCase("South", -2, -1, 2, 1)]
    [TestCase("East", 1, 2, -2, 1)]
    [TestCase("East", -2, -1, 1, -2)]
    [TestCase("West", 1, 2, 2, -1)]
    [TestCase("West", -2, -1, -1, 2)]
    public void RotateTowards_ShouldRotateTheVectorToAlignWithTheDirection(string directionName, int row,
        int column, int expectedRow, int expectedColumn)
    {
        //Arrange
        Direction direction = directionName;
        ICoordinate coordinate = CreateCoordinate(row, column);
        ICoordinate expectedCoordinate = CreateCoordinate(expectedRow, expectedColumn);

        //Act
        ICoordinate result = coordinate.RotateTowards(direction);

        //Assert
        Assert.That(result, Is.EqualTo(expectedCoordinate),
            $"{coordinate} rotated to the {directionName}, should be {expectedCoordinate}");
    }

    [MonitoredTest]
    [TestCase(0, 0, 0, 0, 0)]
    [TestCase(0, 0, 2, 2, 2)]
    [TestCase(1, 1, 2, 1, 1)]
    [TestCase(1, 1, 1, 2, 1)]
    [TestCase(2, 2, 0, 0, 2)]
    public void EXTRA_GetDistanceTo_ShouldGetTheDistanceOfAStraightLineBetweenTheCoordinates(int fromRow,
        int fromColumn, int toRow, int toColumn, int expectedDistance)
    {
        //Arrange
        ICoordinate from = CreateCoordinate(fromRow, fromColumn);
        ICoordinate to = CreateCoordinate(toRow, toColumn);

        //Act
        int distance = from.GetDistanceTo(to);

        //Assert
        Assert.That(distance, Is.EqualTo(expectedDistance),
            $"Distance from {from} to {to} should be {expectedDistance}");
    }

    private ICoordinate CreateCoordinate(int row, int column)
    {
        ICoordinate? coordinate = new Coordinate(row, column) as ICoordinate;
        Assert.That(coordinate, Is.Not.Null, "Coordinate should implement the ICoordinate interface");
        return coordinate;
    }
}