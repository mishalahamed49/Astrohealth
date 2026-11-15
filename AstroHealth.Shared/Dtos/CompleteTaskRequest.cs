using System.ComponentModel.DataAnnotations;

namespace AstroHealth.Shared.Dtos;

public sealed class CompleteTaskRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string TaskName { get; set; } = string.Empty;
}
