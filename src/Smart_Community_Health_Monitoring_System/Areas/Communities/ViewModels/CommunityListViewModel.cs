namespace Smart_Community_Health_Monitoring_System.Areas.Communities.ViewModels;

public class CommunityListViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public int? ApproximatePopulation { get; set; }
    public int? PopulationReferenceYear { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
