﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Onitama.Api.Models;
using Onitama.Api.Models.Output;
using Onitama.Core;
using Onitama.Core.GameAggregate;
using Onitama.Core.TableAggregate;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;

namespace Onitama.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TablesController : ApiControllerBase
{
    private readonly ITableManager _tableManager;
    private readonly ITableRepository _tableRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public TablesController(ITableManager tableManager, ITableRepository tableRepository, IMapper mapper, UserManager<User> userManager)
    {
        _tableManager = tableManager;
        _tableRepository = tableRepository;
        _mapper = mapper;
        _userManager = userManager;
    }

    /// <summary>
    /// Gets a specific table by its id.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TableModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetTableById(Guid id)
    {
        try
        {
            ITable table = _tableRepository.Get(id);
            TableModel model = _mapper.Map<TableModel>(table);
            return Ok(model);
        } catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
        
    }

    /// <summary>
    /// Gets all the tables that are available to join.
    /// </summary>
    [HttpGet("with-available-seats")]
    [ProducesResponseType(typeof(IList<TableModel>), StatusCodes.Status200OK)]
    public IActionResult GetTablesWithAvailableSeats()
    {
        IList<ITable> tables = _tableRepository.FindTablesWithAvailableSeats();
        List<TableModel> models = tables.Select(t => _mapper.Map<TableModel>(t)).ToList();
        return Ok(models);
    }

    /// <summary>
    /// Gets all the tables that are currently active, regardless of available seats.
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(IList<TableModel>), StatusCodes.Status200OK)]
    public IActionResult GetTables()
    {
        IList<ITable> tables = _tableRepository.FindTables();
        List<TableModel> models = tables.Select(t => _mapper.Map<TableModel>(t)).ToList();
        return Ok(models);
    }

    /// <summary>
    /// Adds a new table to the system. The user that creates the table is automatically seated.
    /// </summary>
    /// <param name="preferences">
    /// Contains info about the type of game you want to play.
    /// </param>
    /// <remarks>Tables are automatically removed from the system after 15 minutes.</remarks>
    [HttpPost]
    [ProducesResponseType(typeof(TableModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddNew([FromBody] TablePreferences preferences)
    {
        User currentUser = (await _userManager.GetUserAsync(User))!;
        ITable createdTable = _tableManager.AddNewTableForUser(currentUser, preferences);

        TableModel createdTableModel = _mapper.Map<TableModel>(createdTable);

        return CreatedAtAction(nameof(GetTableById), new { id = createdTable.Id }, createdTableModel);
    }


    /// <summary>
    /// Adds a new competitive table to the system. The user that creates the table is automatically seated.
    /// </summary>
    /// <param name="preferences">
    /// Contains info about the type of game you want to play.
    /// </param>
    /// <remarks>Tables are automatically removed from the system after 15 minutes.</remarks>
    [HttpPost("competitive")]
    [ProducesResponseType(typeof(TableModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddComp([FromBody]  CompTablePreferences preferences)
    {
        User currentUser = (await _userManager.GetUserAsync(User))!;
        ITable createdTable = _tableManager.AddCompTableForUser(currentUser, preferences);

        TableModel createdTableModel = _mapper.Map<TableModel>(createdTable);

        return CreatedAtAction(nameof(GetTableById), new { id = createdTable.Id }, createdTableModel);
    }

    /// <summary>
    /// Adds a new Blitz table to the system. The user that creates the table is automatically seated.
    /// </summary>
    /// <param name="preferences">
    /// Contains info about the type of game you want to play.
    /// </param>
    /// <remarks>Tables are automatically removed from the system after 15 minutes.</remarks>
    [HttpPost("blitz")]
    [ProducesResponseType(typeof(TableModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddBlitz([FromBody] BlitzTablePreferences preferences)
    {
        User currentUser = (await _userManager.GetUserAsync(User))!;
        ITable createdTable = _tableManager.AddBlitzTableForUser(currentUser, preferences);

        TableModel createdTableModel = _mapper.Map<TableModel>(createdTable);

        return CreatedAtAction(nameof(GetTableById), new { id = createdTable.Id }, createdTableModel);
    }

    /// <summary>
    /// Adds a new Custom table to the system. The user that creates the table is automatically seated.
    /// </summary>
    /// <param name="preferences">
    /// Contains info about the type of game you want to play.
    /// </param>
    /// <remarks>Tables are automatically removed from the system after 15 minutes.</remarks>
    [HttpPost("custom")]
    [ProducesResponseType(typeof(TableModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCustom([FromBody] CustomTablePreferences preferences)
    {
        User currentUser = (await _userManager.GetUserAsync(User))!;
        ITable createdTable = _tableManager.AddCustomTableForUser(currentUser, preferences);

        TableModel createdTableModel = _mapper.Map<TableModel>(createdTable);

        return CreatedAtAction(nameof(GetTableById), new { id = createdTable.Id }, createdTableModel);
    }

    /// <summary>
    /// Adds the user to an available seat at an existing table. The user that creates the table is automatically seated.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the table.
    /// </param>
    [HttpPost("{id}/join")]
    [ProducesResponseType(typeof(TableModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Join(Guid id)
    {
        User currentUser = (await _userManager.GetUserAsync(User))!;
        _tableManager.JoinTable(id, currentUser);
        TableModel alteredTableModel = _mapper.Map<TableModel>(_tableRepository.Get(id));
        return Ok(alteredTableModel);
    }

    /// <summary>
    /// Fills the remaining available seats of a table with computer players.
    /// Only the owner of the table is allowed to do this.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the table.
    /// </param>
    [HttpPost("{id}/fill-with-ai")]
    [ProducesResponseType(typeof(TableModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FillWithArtificialPlayers(Guid id)
    {
        User currentUser = (await _userManager.GetUserAsync(User))!;
        _tableManager.FillWithArtificialPlayers(id, currentUser);
        TableModel alteredTableModel = _mapper.Map<TableModel>(_tableRepository.Get(id));
        return Ok(alteredTableModel);
    }

    /// <summary>
    /// Removes the user that is logged in from a table.
    /// If no players are left at the table, the table is removed from the system.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the table.
    /// </param>
    [HttpPost("{id}/leave")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Leave(Guid id)
    {
        User currentUser = (await _userManager.GetUserAsync(User))!;
        _tableManager.LeaveTable(id, currentUser);
        return Ok();
    }

    /// <summary>
    /// Starts a game for the table.
    /// Only the owner of the table can start the game.
    /// A game can only be started if the required number of players are seated.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the table.
    /// </param>
    [HttpPost("{id}/start-game")]
    [ProducesResponseType(typeof(TableModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartGame(Guid id)
    {
        User currentUser = (await _userManager.GetUserAsync(User))!;
        
        _tableManager.StartGameForTable(id, currentUser);
        TableModel alteredTableModel = _mapper.Map<TableModel>(_tableRepository.Get(id));
        return Ok(alteredTableModel);
    }

    [HttpPost("{id}/start-game-ai")]
    [ProducesResponseType(typeof(TableModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartGameAi(Guid id)
    {
        User currentUser = (await _userManager.GetUserAsync(User))!;

        _tableManager.StartGameForTableAi(id, currentUser);
        TableModel alteredTableModel = _mapper.Map<TableModel>(_tableRepository.Get(id));
        return Ok(alteredTableModel);
    }
}