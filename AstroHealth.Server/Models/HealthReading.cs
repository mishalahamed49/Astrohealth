using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AstroHealth.Server.Models;

public sealed class HealthReading
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string AstronautId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int? HeartRate { get; set; }
    public int? BloodPressureSystolic { get; set; }
    public int? BloodPressureDiastolic { get; set; }
    public double? BloodSugar { get; set; }
    public double? BodyTemperature { get; set; }
    public double? SleepHours { get; set; }
    public int? MoodScore { get; set; }
    public double? OxygenSaturation { get; set; }
    public double? RadiationDose { get; set; }
    public string OverallStatus { get; set; } = "Normal";
    public List<string> Alerts { get; set; } = new();
    public bool IsSynthetic { get; set; }
}
