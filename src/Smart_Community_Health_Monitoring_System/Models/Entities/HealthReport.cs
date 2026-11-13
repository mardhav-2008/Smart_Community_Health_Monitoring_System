
namespace Smart_Community_Health_Monitoring_System.Models.Entities;

public class HealthReport
{
    public int Id { get; set; }

    public int CommunityId { get; set; }
    public Community Community { get; set; } = null!;

    public string Category { get; set; } = string.Empty;

    // Aggregated community-level report count.
    public int ReportCount { get; set; }

    public DateTime ReportedAtUtc { get; set; }
    public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;

    public string? Source { get; set; }

    public bool IsVerified { get; set; }
}
