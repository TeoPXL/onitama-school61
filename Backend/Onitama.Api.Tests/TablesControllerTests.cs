using System.Security.Claims;
using AutoMapper;
using Guts.Client.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Onitama.Api.Controllers;
using Onitama.Api.Models.Output;
using Onitama.Core.TableAggregate;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.Tests.Builders;
using Onitama.Core.UserAggregate;

namespace Onitama.Api.Tests;

[ProjectComponentTestFixture("1TINProject", "Onitama", "TablesCtlr", 
    @"Onitama.Api\Controllers\TablesController.cs")]
public class TablesControllerTests
{
    private TablesController _controller = null!;
    private Mock<ITableManager> _tableManagerMock = null!;
    private Mock<ITableRepository> _tableRepositoryMock = null!;
    private Mock<UserManager<User>> _userManagerMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private User _loggedInUser = null!;
    private TablePreferences _tablePreferences = null!;

    [SetUp]
    public void Setup()
    {
        _tableManagerMock = new Mock<ITableManager>();
        _tableRepositoryMock = new Mock<ITableRepository>();
        _mapperMock = new Mock<IMapper>();

        var userStoreMock = new Mock<IUserStore<User>>();
        var passwordHasherMock = new Mock<IPasswordHasher<User>>();
        var lookupNormalizerMock = new Mock<ILookupNormalizer>();
        var errorsMock = new Mock<IdentityErrorDescriber>();
        var loggerMock = new Mock<ILogger<UserManager<User>>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object,
            null,
            passwordHasherMock.Object,
            null,
            null,
            lookupNormalizerMock.Object,
            errorsMock.Object,
            null,
            loggerMock.Object);

        _controller = new TablesController(_tableManagerMock.Object, _tableRepositoryMock.Object, _mapperMock.Object, _userManagerMock.Object);

        _loggedInUser = new UserBuilder().Build();
        var userClaimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _loggedInUser.Id.ToString())
            })
        );
        var context = new ControllerContext { HttpContext = new DefaultHttpContext() };
        context.HttpContext.User = userClaimsPrincipal;
        _controller.ControllerContext = context;
        _userManagerMock.Setup(manager => manager.GetUserAsync(userClaimsPrincipal))
            .ReturnsAsync(_loggedInUser);

        _tablePreferences = new TablePreferencesBuilder().Build();
    }

    [MonitoredTest]
    public void GetTableById_ShouldRetrieveTableUsingRepositoryAndReturnAModelOfIt()
    {
        //Arrange
        ITable table = new TableMockBuilder().Mock.Object;
        _tableRepositoryMock.Setup(repository => repository.Get(table.Id)).Returns(table);

        var tableModel = new TableModel();
        _mapperMock.Setup(mapper => mapper.Map<TableModel>(It.IsAny<ITable>())).Returns(tableModel);

        //Act
        var result = _controller.GetTableById(table.Id) as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
        _mapperMock.Verify(mapper => mapper.Map<TableModel>(table), Times.Once,
            "The table is not correctly mapped to a table model");
        Assert.That(result!.Value, Is.SameAs(tableModel), "The mapped table model is not in the OkObjectResult");
    }

    [MonitoredTest]
    public void GetTablesWithAvailableSeats_ShouldRetrieveAvailableTablesUsingRepositoryAndReturnModelsOfThem()
    {
        //Arrange
        List<ITable> availableTables = [
            new TableMockBuilder().Mock.Object,
            new TableMockBuilder().Mock.Object
        ];

        _tableRepositoryMock.Setup(repository => repository.FindTablesWithAvailableSeats()).Returns(availableTables);

        var tableModel = new TableModel();
        _mapperMock.Setup(mapper => mapper.Map<TableModel>(It.IsAny<ITable>())).Returns(tableModel);

        //Act
        var result = _controller.GetTablesWithAvailableSeats() as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
        _mapperMock.Verify(mapper => mapper.Map<TableModel>(It.IsIn<ITable>(availableTables)),
            Times.Exactly(availableTables.Count), "The tables are not correctly mapped to table models");
        Assert.That(result!.Value, Has.Count.EqualTo(availableTables.Count), "Incorrect amount of table models in the OkObjectResult");
        Assert.That(result!.Value, Has.All.TypeOf<TableModel>(), "The OkObjectResult should hold a collection of table models");
    }

    [MonitoredTest]
    public void AddNew_ShouldUseTheTableManagerToAddANewTableSeatedByTheLoggedInUser()
    {
        //Arrange
        ITable createdTable = new TableMockBuilder().Object;
        _tableManagerMock.Setup(manager => manager.AddNewTableForUser(It.IsAny<User>(), It.IsAny<TablePreferences>()))
            .Returns(createdTable);

        var tableModel = new TableModel();
        _mapperMock.Setup(mapper => mapper.Map<TableModel>(It.IsAny<ITable>())).Returns(tableModel);

        //Act
        var result = _controller.AddNew(_tablePreferences).Result as CreatedAtActionResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'CreatedAtActionResult' should be returned.");

        _userManagerMock.Verify(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once,
            "The 'GetUserAsync' of the UserManager is not called");

        _tableManagerMock.Verify(manager => manager.AddNewTableForUser(_loggedInUser, _tablePreferences), Times.Once,
            "The 'AddNewTableForUser' method of the table manager is not called correctly");

        _mapperMock.Verify(mapper => mapper.Map<TableModel>(createdTable), Times.Once,
            "The table is not correctly mapped to a table model");

        Assert.That(result!.Value, Is.SameAs(tableModel), "The mapped table model is not in the CreatedAtActionResult");
        Assert.That(result.RouteValues["id"], Is.EqualTo(createdTable.Id), "The id of the created table is not in the route values");
        Assert.That(result.ActionName, Is.EqualTo(nameof(TablesController.GetTableById)), "The action name of the CreatedAtActionResult is not correct");
    }

    [MonitoredTest]
    public void Join_ShouldUseTheTableManagerToAddTheLoggedInUserToTheTable()
    {
        //Arrange
        ITable existingTable = new TableMockBuilder().Object;
        _tableRepositoryMock.Setup(repository => repository.Get(existingTable.Id)).Returns(existingTable);

        var tableModel = new TableModel();
        _mapperMock.Setup(mapper => mapper.Map<TableModel>(It.IsAny<ITable>())).Returns(tableModel);

        //Act
        var result = _controller.Join(existingTable.Id).Result as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _userManagerMock.Verify(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once,
            "The 'GetUserAsync' of the UserManager is not called");

        _tableManagerMock.Verify(manager => manager.JoinTable(existingTable.Id, _loggedInUser), Times.Once,
            "The 'JoinTable' method of the table manager is not called correctly");

        _tableRepositoryMock.Verify(repository => repository.Get(existingTable.Id), Times.Once,
            "The table should be retrieved with the repository (to be able to map it to a table model)");

        _mapperMock.Verify(mapper => mapper.Map<TableModel>(existingTable), Times.Once,
            "The table is not correctly mapped to a table model");

        Assert.That(result!.Value, Is.SameAs(tableModel), "The mapped table model is not in the OkObjectResult");
    }

    [MonitoredTest]
    public void Leave_ShouldUseTheTableManagerToRemoveTheLoggedInUserFromTheTable()
    {
        //Arrange
        ITable existingTable = new TableMockBuilder().Object;

        //Act
        var result = _controller.Leave(existingTable.Id).Result as OkResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkResult' should be returned.");

        _userManagerMock.Verify(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once,
            "The 'GetUserAsync' of the UserManager is not called");

        _tableManagerMock.Verify(manager => manager.LeaveTable(existingTable.Id, _loggedInUser), Times.Once,
            "The 'LeaveTable' method of the table manager is not called correctly");
    }

    [MonitoredTest]
    public void StartGame_ShouldUseTheTableManagerStartAGameForTheTable()
    {
        //Arrange
        ITable existingTable = new TableMockBuilder().Object;
        _tableRepositoryMock.Setup(repository => repository.Get(existingTable.Id)).Returns(existingTable);

        var tableModel = new TableModel();
        _mapperMock.Setup(mapper => mapper.Map<TableModel>(It.IsAny<ITable>())).Returns(tableModel);

        //Act
        var result = _controller.StartGame(existingTable.Id).Result as OkObjectResult;

        //Assert
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _userManagerMock.Verify(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once,
            "The 'GetUserAsync' of the UserManager is not called");

        _tableManagerMock.Verify(manager => manager.StartGameForTable(existingTable.Id, _loggedInUser), Times.Once,
            "The 'StartGameForTable' method of the table manager is not called correctly");

        _tableRepositoryMock.Verify(repository => repository.Get(existingTable.Id), Times.Once,
            "The table should be retrieved with the repository (to be able to map it to a table model)");

        _mapperMock.Verify(mapper => mapper.Map<TableModel>(existingTable), Times.Once,
            "The table is not correctly mapped to a table model");

        Assert.That(result!.Value, Is.SameAs(tableModel), "The mapped table model is not in the OkObjectResult");
    }
}