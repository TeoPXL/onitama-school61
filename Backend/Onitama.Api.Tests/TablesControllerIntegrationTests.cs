using System.Collections;
using System.Security.Claims;
using AutoMapper;
using Guts.Client.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Onitama.Api.Controllers;
using Onitama.Api.Models.Input;
using Onitama.Api.Models.Output;
using Onitama.Api.Tests.Util;
using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.TableAggregate;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.Tests.Builders;
using Onitama.Core.UserAggregate;
using Onitama.Infrastructure;

namespace Onitama.Api.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Onitama", "TablesIntegration",
        @"Onitama.Api\Controllers\TablesController.cs;
Onitama.Core\TableAggregate\TableManager.cs;
Onitama.Core\TableAggregate\TableFactory.cs;
Onitama.Core\TableAggregate\TablePreferences.cs;
Onitama.Core\TableAggregate\Table.cs;
Onitama.Infrastructure\InMemoryTableRepository.cs;")]
    public class TablesControllerIntegrationTests : ControllerIntegrationTestsBase<TablesController>
    {
        [MonitoredTest]
        public void HappyFlow_UserAStartsATable_UserBJoins_UserAStartsTheGame()
        {
            StartANewGameForANewTable();
        }
    }
}