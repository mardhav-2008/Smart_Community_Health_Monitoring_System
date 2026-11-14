
namespace Smart_Community_Health_Monitoring_System.Models.Entities;

public class WaterSource
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public bool IsActive { get; set; } = true;

    public int CommunityId { get; set; }
    public Community Community { get; set; } = null!;

    public ICollection<WaterObservation> Observations { get; set; } = [];
}
