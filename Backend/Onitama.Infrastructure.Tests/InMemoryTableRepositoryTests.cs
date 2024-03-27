using Guts.Client.Core;
using Moq;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.Tests.Builders;

namespace Onitama.Infrastructure.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Onitama", "InMemoryTableRepo",
        @"Onitama.Infrastructure\InMemoryTableRepository.cs")]
    public class InMemoryTableRepositoryTests
    {
        private InMemoryTableRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _repository = new InMemoryTableRepository();
        }

        [MonitoredTest]
        public void FindTablesWithAvailableSeats_SomeTablesHaveAvailableSeats_ShouldReturnThem()
        {
            //Arrange
            int numberOfEmptyTables = Random.Shared.Next(1, 11);
            for (int i = 0; i < numberOfEmptyTables; i++)
            {
                AddEmptyTable();
            }
            int numberOfTablesWithOnePlayer = Random.Shared.Next(1, 11);
            for (int i = 0; i < numberOfTablesWithOnePlayer; i++)
            {
                AddTableWithOnePlayer();
            }
            int numberOfFullTables = Random.Shared.Next(1, 11);
            for (int i = 0; i < numberOfFullTables; i++)
            {
                AddFullTable();
            }

            //Act
            IList<ITable>? results = _repository.FindTablesWithAvailableSeats();

            //Assert
            Assert.That(results, Is.Not.Null, "No list was returned.");
            foreach (ITable table in results)
            {
                Assert.That(table.HasAvailableSeat, Is.True, "Not all returned tables have seats available");
            }
            Assert.That(results.Count, Is.EqualTo(numberOfEmptyTables + numberOfTablesWithOnePlayer), "The number of returned tables is incorrect.");
        }

        [MonitoredTest]
        public void FindTablesWithAvailableSeats_AllTablesAreFull_ShouldReturnAnEmptyList()
        {
            //Arrange
            int numberOfFullTables = Random.Shared.Next(3, 11);
            for (int i = 0; i < numberOfFullTables; i++)
            {
                AddFullTable();
            }

            //Act
            IList<ITable>? results = _repository.FindTablesWithAvailableSeats();

            //Assert
            Assert.That(results, Is.Not.Null, "No list was returned.");
            Assert.That(results, Is.Empty, "The list returned should be empty.");
        }

        private void AddEmptyTable()
        {
            _repository.Add(new TableMockBuilder().Object);
        }

        private void AddTableWithOnePlayer()
        {
            _repository.Add(new TableMockBuilder().WithSeatedUsers([new UserBuilder().Build()]).Object);
        }

        private void AddFullTable()
        {
            _repository.Add(new TableMockBuilder().WithSeatedUsers([new UserBuilder().Build(), new UserBuilder().Build()]).Object);
        }
    }
}