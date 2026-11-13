
namespace Smart_Community_Health_Monitoring_System.Models.Entities;

public class Community
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<WaterSource> WaterSources { get; set; } = [];
    public ICollection<HealthReport> HealthReports { get; set; } = [];
    public ICollection<EarlyWarningAlert> Alerts { get; set; } = [];
}
