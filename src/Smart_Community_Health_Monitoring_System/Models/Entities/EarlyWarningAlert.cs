
namespace Smart_Community_Health_Monitoring_System.Models.Entities;

public enum AlertSeverity
{
    Informational,
    Low,
    Moderate,
    High,
    Critical
}

public enum AlertStatus
{
    New,
    UnderReview,
    Confirmed,
    Dismissed,
    Resolved
}

public class EarlyWarningAlert
{
    public int Id { get; set; }

    public int CommunityId { get; set; }
    public Community Community { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public AlertSeverity Severity { get; set; }
    public AlertStatus Status { get; set; } = AlertStatus.New;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAtUtc { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
}
