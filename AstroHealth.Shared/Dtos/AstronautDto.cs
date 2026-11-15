using System.ComponentModel.DataAnnotations;

namespace AstroHealth.Shared.Dtos;

public sealed class AstronautDto
{
    public string? Id { get; set; }

    public string AstronautId { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Role { get; set; } = string.Empty;

    public DateTime MissionStartDate { get; set; } = DateTime.UtcNow;

    [Required]
    [RegularExpression("ISS|Lunar|Mars")]
    public string MissionEnvironment { get; set; } = "ISS";

    public int GamificationPoints { get; set; }

    public List<string> CompletedTasks { get; set; } = new();
}
