namespace Smart_Community_Health_Monitoring_System.Areas.Communities.ViewModels;

public class CommunityDetailsViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string? Description { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public int? ApproximatePopulation { get; set; }
    public int? PopulationReferenceYear { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public int TotalWaterSources { get; set; }
    public int ActiveWaterSources { get; set; }
    public int TotalHealthReports { get; set; }
    public int TotalReportedCases { get; set; }
    public int TotalAlerts { get; set; }
    public int ActiveAlerts { get; set; }

    public List<WaterSourceDetailsViewModel> WaterSources { get; set; } = [];
    public List<HealthReportDetailsViewModel> HealthReports { get; set; } = [];
    public List<AlertDetailsViewModel> Alerts { get; set; } = [];
}

public class WaterSourceDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int ObservationCount { get; set; }
    public DateTime? LastObservedAtUtc { get; set; }
}

public class HealthReportDetailsViewModel
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public int ReportCount { get; set; }
    public DateTime ReportedAtUtc { get; set; }
    public string? Source { get; set; }
    public bool IsVerified { get; set; }
}

public class AlertDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
