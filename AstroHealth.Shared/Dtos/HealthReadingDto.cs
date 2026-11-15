using System.ComponentModel.DataAnnotations;

namespace AstroHealth.Shared.Dtos;

public sealed class HealthReadingDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string AstronautId { get; set; } = "demo-astronaut-001";

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Range(1, 250)]
    public int? HeartRate { get; set; }

    [Range(1, 300)]
    public int? BloodPressureSystolic { get; set; }

    [Range(1, 200)]
    public int? BloodPressureDiastolic { get; set; }

    [Range(1, 1000)]
    public double? BloodSugar { get; set; }

    [Range(25, 45)]
    public double? BodyTemperature { get; set; }

    [Range(0, 24)]
    public double? SleepHours { get; set; }

    [Range(1, 10)]
    public int? MoodScore { get; set; }

    [Range(0, 100)]
    public double? OxygenSaturation { get; set; }

    [Range(0, double.MaxValue)]
    public double? RadiationDose { get; set; }

    public string OverallStatus { get; set; } = "Normal";

    public List<string> Alerts { get; set; } = new();

    public bool IsSynthetic { get; set; }
}
