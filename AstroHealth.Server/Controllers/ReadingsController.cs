using AstroHealth.Server.Models;
using AstroHealth.Server.Services;
using AstroHealth.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AstroHealth.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReadingsController : ControllerBase
{
    private readonly MongoDbService _mongo;
    private readonly ThresholdEvaluator _evaluator;

    public ReadingsController(MongoDbService mongo, ThresholdEvaluator evaluator)
    {
        _mongo = mongo;
        _evaluator = evaluator;
    }

    [HttpGet("{astronautId}")]
    [ProducesResponseType(typeof(IReadOnlyList<HealthReadingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<HealthReadingDto>>> GetReadings(
        string astronautId,
        [FromQuery] int limit = 30)
    {
        if (limit is < 1 or > 100)
            return BadRequest("Limit must be between 1 and 100.");

        var readings = await _mongo.Readings
            .Find(reading => reading.AstronautId == astronautId)
            .SortByDescending(reading => reading.Timestamp)
            .Limit(limit)
            .ToListAsync();

        return Ok(readings.Select(ToDto).ToList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(HealthReadingDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<HealthReadingDto>> AddReading([FromBody] HealthReadingDto dto)
    {
        if (dto.HeartRate is null
            && dto.BloodPressureSystolic is null
            && dto.BloodPressureDiastolic is null
            && dto.BloodSugar is null
            && dto.BodyTemperature is null
            && dto.SleepHours is null
            && dto.MoodScore is null
            && dto.OxygenSaturation is null
            && dto.RadiationDose is null)
        {
            return BadRequest("At least one health measurement is required.");
        }

        var reading = new HealthReading
        {
            AstronautId = dto.AstronautId.Trim(),
            Timestamp = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp,
            HeartRate = dto.HeartRate,
            BloodPressureSystolic = dto.BloodPressureSystolic,
            BloodPressureDiastolic = dto.BloodPressureDiastolic,
            BloodSugar = dto.BloodSugar,
            BodyTemperature = dto.BodyTemperature,
            SleepHours = dto.SleepHours,
            MoodScore = dto.MoodScore,
            OxygenSaturation = dto.OxygenSaturation,
            RadiationDose = dto.RadiationDose
        };

        _evaluator.Evaluate(reading);
        await _mongo.Readings.InsertOneAsync(reading);

        var result = ToDto(reading);
        return CreatedAtAction(nameof(GetLatest), new { astronautId = reading.AstronautId }, result);
    }

    [HttpGet("{astronautId}/latest")]
    [ProducesResponseType(typeof(HealthReadingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HealthReadingDto>> GetLatest(string astronautId)
    {
        var latest = await _mongo.Readings
            .Find(reading => reading.AstronautId == astronautId)
            .SortByDescending(reading => reading.Timestamp)
            .FirstOrDefaultAsync();

        return latest is null ? NotFound() : Ok(ToDto(latest));
    }

    private static HealthReadingDto ToDto(HealthReading reading) => new()
    {
        AstronautId = reading.AstronautId,
        Timestamp = reading.Timestamp,
        HeartRate = reading.HeartRate,
        BloodPressureSystolic = reading.BloodPressureSystolic,
        BloodPressureDiastolic = reading.BloodPressureDiastolic,
        BloodSugar = reading.BloodSugar,
        BodyTemperature = reading.BodyTemperature,
        SleepHours = reading.SleepHours,
        MoodScore = reading.MoodScore,
        OxygenSaturation = reading.OxygenSaturation,
        RadiationDose = reading.RadiationDose,
        OverallStatus = reading.OverallStatus,
        Alerts = reading.Alerts,
        IsSynthetic = reading.IsSynthetic
    };
}
