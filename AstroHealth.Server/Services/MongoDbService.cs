using AstroHealth.Server.Models;
using MongoDB.Driver;

namespace AstroHealth.Server.Services;

public sealed class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];

        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException(
                "MongoDB:ConnectionString and MongoDB:DatabaseName must be configured.");
        }

        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
        settings.ConnectTimeout = TimeSpan.FromSeconds(30);

        var client = new MongoClient(settings);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<HealthReading> Readings =>
        _database.GetCollection<HealthReading>("health_readings");

    public IMongoCollection<Astronaut> Astronauts =>
        _database.GetCollection<Astronaut>("astronauts");
}
