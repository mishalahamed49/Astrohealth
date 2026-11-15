namespace AstroHealth.Client.Services;

public sealed class AlertService
{
    public string GetStatusClass(string? status) => status switch
    {
        "Critical" => "critical",
        "Warning" => "warning",
        "Normal" => "normal",
        _ => "unknown"
    };
}
