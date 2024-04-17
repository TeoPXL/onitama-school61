using Guts.Client.Core;
using Onitama.Core.Util.Contracts;
using Onitama.Core.Util;

namespace Onitama.Core.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "Direction",
    @"Onitama.Core\Util\Direction.cs;")]
public class DirectionTests
{
    [MonitoredTest]
    [TestCase("North", 5, ExpectedResult = "(0, 2)")]
    [TestCase("South", 5, ExpectedResult = "(4, 2)")]
    [TestCase("NorthEast", 5, ExpectedResult = "(0, 0)")]
    [TestCase("NorthWest", 5, ExpectedResult = "(0, 4)")]
    [TestCase("SouthWest", 5, ExpectedResult = "(4, 4)")]
    [TestCase("SouthEast", 5, ExpectedResult = "(4, 0)")]
    public string GetStartCoordinate_ShouldReturnCorrectCoordinate(string directionAsText, int playMatSize)
    {
        // Arrange
        Direction direction = directionAsText;

        // Act
        ICoordinate result = direction.GetStartCoordinate(playMatSize);

        // Assert
        return result?.ToString() ?? string.Empty;
    }

    [MonitoredTest]
    [TestCase("North", "East")]
    [TestCase("East", "South")]
    [TestCase("South", "West")]
    [TestCase("West", "North")]
    [TestCase("NorthWest", "NorthEast")]
    [TestCase("NorthEast", "SouthEast")]
    public void PerpendicularDirection_ShouldReturnCorrectDirection(string directionAsText, string expectedDirectionAsText)
    {
        // Arrange
        Direction direction = directionAsText;

        // Act
        Direction result = direction.PerpendicularDirection;

        // Assert
        Assert.That(expectedDirectionAsText, Is.EqualTo(result.ToString()));
    }

    [MonitoredTest]
    [TestCase("North", "East", "NorthEast")]
    [TestCase("East", "South", "SouthEast")]
    [TestCase("South", "West", "SouthWest")]
    [TestCase("West", "North", "NorthWest")]
    [TestCase("North", "West", "NorthWest")]
    [TestCase("East", "North", "NorthEast")]
    public void CombineWith_ShouldReturnCorrectDirection(string direction1AsText, string direction2AsText, string expectedDirectionAsText)
    {
        // Arrange
        Direction direction1 = direction1AsText;
        Direction direction2 = direction2AsText;

        // Act
        Direction result = direction1.CombineWith(direction2);

        // Assert
        Assert.That(expectedDirectionAsText, Is.EqualTo(result.ToString()));
    }

}