
namespace Smart_Community_Health_Monitoring_System.Models.Entities;

public class Community
{
    public int Id { get; set; }

    // Basic information
    public string Name { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Geographic information
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Population reference
    public int? ApproximatePopulation { get; set; }
    public int? PopulationReferenceYear { get; set; }

    // Registry status
    public bool IsArchived { get; set; } = false;
    public DateTime? ArchivedAtUtc { get; set; }

    // Registration metadata
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<WaterSource> WaterSources { get; set; } = [];
    public ICollection<HealthReport> HealthReports { get; set; } = [];
    public ICollection<EarlyWarningAlert> Alerts { get; set; } = [];
}
