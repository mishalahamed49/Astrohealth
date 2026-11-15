using AstroHealth.Server.Models;
using MongoDB.Driver;

namespace AstroHealth.Server.Services;

public sealed class DemoDataSeeder(
    MongoDbService mongo,
    ThresholdEvaluator evaluator,
    IConfiguration configuration,
    ILogger<DemoDataSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var astronautId = configuration["SeedData:AstronautId"];
        if (string.IsNullOrWhiteSpace(astronautId))
            throw new InvalidOperationException("SeedData:AstronautId must be configured.");

        await EnsureDemoAstronautAsync(astronautId, cancellationToken);

        var existingReading = await mongo.Readings
            .Find(reading => reading.AstronautId == astronautId)
            .Limit(1)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingReading is not null)
        {
            logger.LogInformation(
                "Skipped demo health data seed because readings already exist for {AstronautId}.",
                astronautId);
            return;
        }

        var now = DateTime.UtcNow;
        var readings = Enumerable.Range(0, 30)
            .Select(index => CreateReading(astronautId, now.AddDays(index - 29), index))
            .ToList();

        await mongo.Readings.InsertManyAsync(readings, cancellationToken: cancellationToken);

        logger.LogInformation(
            "Seeded {Count} synthetic health readings for demo astronaut {AstronautId}.",
            readings.Count,
            astronautId);
    }

    private async Task EnsureDemoAstronautAsync(string astronautId, CancellationToken cancellationToken)
    {
        var existingAstronaut = await mongo.Astronauts
            .Find(astronaut => astronaut.AstronautId == astronautId)
            .Limit(1)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingAstronaut is not null)
            return;

        var astronaut = new Astronaut
        {
            AstronautId = astronautId,
            Name = "Demo Commander",
            Role = "Mission Specialist",
            MissionStartDate = DateTime.UtcNow.AddDays(-142),
            MissionEnvironment = "ISS"
        };

        await mongo.Astronauts.InsertOneAsync(astronaut, cancellationToken: cancellationToken);
        logger.LogInformation("Created the synthetic demo astronaut profile {AstronautId}.", astronautId);
    }

    private HealthReading CreateReading(string astronautId, DateTime timestamp, int index)
    {
        var reading = new HealthReading
        {
            AstronautId = astronautId,
            Timestamp = timestamp,
            HeartRate = index == 19 ? 114 : 64 + (index * 7 % 19),
            BloodPressureSystolic = 108 + (index * 7 % 23),
            BloodPressureDiastolic = 68 + (index * 3 % 14),
            BloodSugar = 84 + (index * 11 % 33),
            BodyTemperature = 36.3 + (index % 8 * 0.1),
            SleepHours = index == 11 ? 5.2 : 6.4 + (index * 3 % 12 * 0.1),
            MoodScore = index == 11 ? 3 : 6 + index % 4,
            OxygenSaturation = 96 + index % 4,
            RadiationDose = 9 + index * 0.16,
            IsSynthetic = true
        };

        evaluator.Evaluate(reading);
        return reading;
    }
}
