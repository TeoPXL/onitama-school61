using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onitama.Api.Models;
using Onitama.Api.Models.Output;

namespace Onitama.Api.Controllers;

//DO NOT TOUCH THIS FILE!!
[Route("")]
[AllowAnonymous]
public class HomeController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectPermanent("~/swagger");
    }

    [HttpGet("ping")]
    [ProducesResponseType(typeof(PingResultModel), (int)HttpStatusCode.OK)]
    public IActionResult Ping()
    {
        var model = new PingResultModel
        {
            IsAlive = true,
            Greeting = $"Hello on this fine {DateTime.Now.DayOfWeek}"
        };
        return Ok(model);
    }
}