
namespace Smart_Community_Health_Monitoring_System.Models;

public class MonitoringDashboardViewModel
{
    public string ProjectStatus { get; set; } = "DEMONSTRATION";

    public DateTime LastUpdated { get; set; }

    public int MonitoredWaterSources { get; set; }

    public int CommunityReports { get; set; }

    public int ActiveAlerts { get; set; }

    public List<WaterObservation> WaterObservations { get; set; } = [];

    public List<HealthSummary> HealthSummaries { get; set; } = [];
}

public class WaterObservation
{
    public string Location { get; set; } = "";

    public string Parameter { get; set; } = "";

    public string Reading { get; set; } = "";

    public string Unit { get; set; } = "";

    public string Status { get; set; } = "";
}

public class HealthSummary
{
    public string Category { get; set; } = "";

    public int Reports { get; set; }
}
