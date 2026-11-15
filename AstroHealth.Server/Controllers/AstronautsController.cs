using AstroHealth.Server.Models;
using AstroHealth.Server.Services;
using AstroHealth.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AstroHealth.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AstronautsController : ControllerBase
{
    private readonly MongoDbService _mongo;

    public AstronautsController(MongoDbService mongo)
    {
        _mongo = mongo;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AstronautDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AstronautDto>>> GetAstronauts()
    {
        var astronauts = await _mongo.Astronauts
            .Find(Builders<Astronaut>.Filter.Empty)
            .SortBy(astronaut => astronaut.Name)
            .ToListAsync();

        return Ok(astronauts.Select(ToDto).ToList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(AstronautDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AstronautDto>> CreateAstronaut([FromBody] AstronautDto dto)
    {
        var astronaut = new Astronaut
        {
            AstronautId = Guid.NewGuid().ToString("N"),
            Name = dto.Name.Trim(),
            Role = dto.Role.Trim(),
            MissionStartDate = dto.MissionStartDate,
            MissionEnvironment = dto.MissionEnvironment
        };

        await _mongo.Astronauts.InsertOneAsync(astronaut);
        var result = ToDto(astronaut);
        return CreatedAtAction(nameof(GetAstronauts), result);
    }

    [HttpPost("{id}/complete-task")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTask(string id, [FromBody] CompleteTaskRequest request)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(id, out _))
            return NotFound();

        var update = Builders<Astronaut>.Update
            .Inc(astronaut => astronaut.GamificationPoints, 10)
            .Push(astronaut => astronaut.CompletedTasks, request.TaskName.Trim());

        var result = await _mongo.Astronauts.UpdateOneAsync(astronaut => astronaut.Id == id, update);
        return result.MatchedCount == 0 ? NotFound() : Ok();
    }

    private static AstronautDto ToDto(Astronaut astronaut) => new()
    {
        Id = astronaut.Id,
        AstronautId = astronaut.AstronautId,
        Name = astronaut.Name,
        Role = astronaut.Role,
        MissionStartDate = astronaut.MissionStartDate,
        MissionEnvironment = astronaut.MissionEnvironment,
        GamificationPoints = astronaut.GamificationPoints,
        CompletedTasks = astronaut.CompletedTasks
    };
}
