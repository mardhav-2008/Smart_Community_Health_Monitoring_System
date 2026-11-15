using System.ComponentModel.DataAnnotations;

namespace Smart_Community_Health_Monitoring_System.Areas.Communities.ViewModels;

public class CommunityEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string District { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(-90, 90)]
    public double? Latitude { get; set; }

    [Range(-180, 180)]
    public double? Longitude { get; set; }

    [Range(1, int.MaxValue)]
    public int? ApproximatePopulation { get; set; }

    [Range(1900, 2100)]
    public int? PopulationReferenceYear { get; set; }
}
