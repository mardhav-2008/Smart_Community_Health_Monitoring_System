
namespace Smart_Community_Health_Monitoring_System.Models.Entities;

public class WaterObservation
{
    public int Id { get; set; }

    public int WaterSourceId { get; set; }
    public WaterSource WaterSource { get; set; } = null!;

    public string Parameter { get; set; } = string.Empty;

    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;

    public DateTime ObservedAtUtc { get; set; }
    public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;

    public bool IsValidated { get; set; }
}
