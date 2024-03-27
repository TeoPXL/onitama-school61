using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Onitama.Api.Controllers;
using Onitama.Api.Models.Input;
using Onitama.Api.Models.Output;
using Onitama.Api.Tests.Util;
using Onitama.Api.Util;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.TableAggregate;
using Onitama.Core.UserAggregate;
using Onitama.Infrastructure;

namespace Onitama.Api.Tests;

public abstract class ControllerIntegrationTestsBase<TController> where TController : ApiControllerBase
{
    protected HttpClient ClientA = null!;
    protected HttpClient ClientB = null!;
    protected JsonSerializerOptions JsonSerializerOptions = null!;
    protected AccessPassModel WarriorAAccessPass = null!;
    protected AccessPassModel WarriorBAccessPass = null!;

    [OneTimeSetUp]
    public void BeforeAllTests()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        JsonSerializerOptions.Converters.Add(new TwoDimensionalArrayJsonConverter<MoveCardGridCellType>());
        JsonSerializerOptions.Converters.Add(new TwoDimensionalArrayJsonConverter<PawnModel>());

        var factory = new TestWebApplicationFactory(services =>
        {
            // Replace the database with an in-memory database
            services.AddDbContext<OnitamaDbContext>(options =>
            {
                options.UseSqlite("Filename=:memory:").EnableSensitiveDataLogging(true);
            });
        });
        ClientA = factory.CreateClient();
        ClientB = factory.CreateClient();

        WarriorAAccessPass = RegisterAndLoginUser(ClientA, "WarriorA");
        WarriorBAccessPass = RegisterAndLoginUser(ClientB, "WarriorB");
    }

    [OneTimeTearDown]
    public void AfterAllTests()
    {
        ClientA.Dispose();
        ClientB.Dispose();
    }

    private AccessPassModel RegisterAndLoginUser(HttpClient client, string warriorName)
    {
        var registerModel = new RegisterModel
        {
            Email = $"{warriorName}@test.be",
            Password = "password",
            WariorName = warriorName
        };
        client.PostAsJsonAsync("api/authentication/register",
                registerModel)
            .Wait();
        HttpResponseMessage response = client.PostAsJsonAsync("api/authentication/token", new LoginModel
        {
            Email = registerModel.Email,
            Password = registerModel.Password
        }).Result!;

        AccessPassModel accessPassModel = response.Content.ReadAsAsync<AccessPassModel>().Result;
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessPassModel.Token);
        return accessPassModel;
    }

    protected TableModel StartANewGameForANewTable()
    {
        //User A starts a table
        HttpResponseMessage response = ClientA.PostAsJsonAsync("api/tables", new TablePreferences()).Result;
        Assert.That((int)response.StatusCode, Is.EqualTo(StatusCodes.Status201Created), "User A could not correctly add a table.");
        TableModel table = response.Content.ReadAsAsync<TableModel>().Result;
        Assert.That(table, Is.Not.Null, "User A could not correctly add a table.");
        Assert.That(table.SeatedPlayers.Count, Is.EqualTo(1), "User A could not correctly add a table. There should be 1 seated player");
        Assert.That(table.SeatedPlayers.First().Name, Is.EqualTo(WarriorAAccessPass.User.WarriorName),
            "User A could not correctly add a table. The seated player has an incorrect warrior name");
        Assert.That(table.SeatedPlayers.First().Id, Is.EqualTo(WarriorAAccessPass.User.Id),
            "User A could not correctly add a table. The seated player has an incorrect id (should be the id of the user");
        Assert.That(table.OwnerPlayerId, Is.EqualTo(WarriorAAccessPass.User.Id),
            "User A could not correctly add a table. The owner of the table has an incorrect id (should be the id of the user)");
        Assert.That(table.GameId, Is.EqualTo(Guid.Empty),
            "User A could not correctly add a table. The GameId of the new table should be an empty Guid.");
        Assert.That(table.HasAvailableSeat, Is.True,
            "User A could not correctly add a table. The table should have available seats left.");

        //User B finds a table with available seats
        response = ClientB.GetAsync("api/tables/with-available-seats").Result;
        Assert.That((int)response.StatusCode, Is.EqualTo(StatusCodes.Status200OK), "User B could not correctly get tables with available seats.");
        IList<TableModel> availableTableModels = response.Content.ReadAsAsync<List<TableModel>>().Result;
        Assert.That(availableTableModels!.Count, Is.EqualTo(1), "User B could not correctly get tables with available seats.");
        table = availableTableModels.First();

        //User B joins the table
        response = ClientB.PostAsync($"api/tables/{table.Id}/join", null).Result;
        Assert.That((int)response.StatusCode, Is.EqualTo(StatusCodes.Status200OK), "User B could not correctly join the available table.");
        table = response.Content.ReadAsAsync<TableModel>().Result;
        Assert.That(table, Is.Not.Null, "User B could not correctly join the available table.");
        Assert.That(table.SeatedPlayers.Count, Is.EqualTo(2),
            "User B could not correctly join the available table. There should be 2 seated players");
        Assert.That(table.SeatedPlayers.First().Color, Is.Not.EqualTo(table.SeatedPlayers.Last().Color),
                       "User B could not correctly join the available table. The seated players should have different colors");
        Assert.That(table.HasAvailableSeat, Is.False,
            "User B could not correctly join the available table. The table should not have any available seats left.");

        //User A starts the game
        response = ClientA.PostAsync($"api/tables/{table.Id}/start-game", null).Result;
        Assert.That((int)response.StatusCode, Is.EqualTo(StatusCodes.Status200OK), "User A could not correctly start the game.");
        table = response.Content.ReadAsAsync<TableModel>().Result;
        Assert.That(table, Is.Not.Null, "User a could not correctly start the game.");
        Assert.That(table!.GameId, Is.Not.EqualTo(Guid.Empty),
            "User A could not correctly start the game. Game Id is empty");
        return table;
    }
}