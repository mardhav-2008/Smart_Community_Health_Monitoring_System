namespace Smart_Community_Health_Monitoring_System.Areas.Communities.ViewModels;

public class CommunityDirectoryViewModel
{
    // Community records for the current page.
    public List<CommunityListViewModel> Communities { get; set; } = [];

    // Search and filters.
    public string? SearchTerm { get; set; }
    public string? SelectedState { get; set; }
    public string? SelectedDistrict { get; set; }
    public int? MinimumPopulation { get; set; }
    public int? MaximumPopulation { get; set; }
    public bool ShowArchived { get; set; }

    // Sorting.
    public string SortBy { get; set; } = "name";
    public bool SortDescending { get; set; }

    // Pagination.
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public bool HasMore { get; set; }

    // Directory statistics.
    public int TotalCommunities { get; set; }
    public int ActiveCommunities { get; set; }
    public int ArchivedCommunities { get; set; }

    // Available filter options.
    public List<string> AvailableStates { get; set; } = [];
    public List<string> AvailableDistricts { get; set; } = [];
}
