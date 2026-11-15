
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Community_Health_Monitoring_System.Areas.Communities.ViewModels;
using Smart_Community_Health_Monitoring_System.Data;
using Smart_Community_Health_Monitoring_System.Models.Entities;

namespace Smart_Community_Health_Monitoring_System.Areas.Communities.Controllers;

[Area("Communities")]
[Authorize(Policy = "FieldWorkerOrAdministrator")]
public class CommunitiesController : Controller
{
    private readonly ApplicationDbContext _context;

    private const int PageSize = 12;

    public CommunitiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Communities/Communities
    [HttpGet]
    public async Task<IActionResult> Index(
        string? search,
        string? state,
        string? district,
        int? minPopulation,
        int? maxPopulation,
        bool archived = false,
        string sortBy = "name",
        bool descending = false,
        int page = 1)
    {
        page = Math.Max(1, page);

        // FieldWorkers cannot browse archived communities.
        if (archived && !User.IsInRole("Administrator"))
            archived = false;

        var query = _context.Communities
            .AsNoTracking()
            .AsQueryable();

        // Archive filter.
        query = query.Where(
            community => community.IsArchived == archived);

        // Search by name, district or state.
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(community =>
                community.Name.Contains(term) ||
                community.District.Contains(term) ||
                community.State.Contains(term));
        }

        // State filter.
        if (!string.IsNullOrWhiteSpace(state))
        {
            query = query.Where(
                community => community.State == state);
        }

        // District filter.
        if (!string.IsNullOrWhiteSpace(district))
        {
            query = query.Where(
                community => community.District == district);
        }

        // Population range.
        if (minPopulation.HasValue)
        {
            query = query.Where(community =>
                community.ApproximatePopulation >= minPopulation);
        }

        if (maxPopulation.HasValue)
        {
            query = query.Where(community =>
                community.ApproximatePopulation <= maxPopulation);
        }

        // Sorting.
        query = sortBy.ToLowerInvariant() switch
        {
            "population" => descending
                ? query.OrderByDescending(c => c.ApproximatePopulation)
                : query.OrderBy(c => c.ApproximatePopulation),

            "date" => descending
                ? query.OrderByDescending(c => c.CreatedAtUtc)
                : query.OrderBy(c => c.CreatedAtUtc),

            _ => descending
                ? query.OrderByDescending(c => c.Name)
                : query.OrderBy(c => c.Name)
        };

        var totalCommunities = await query.CountAsync();

        // Fetch one extra record to determine whether another page exists.
        var communities = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize + 1)
            .Select(community => new CommunityListViewModel
            {
                Id = community.Id,
                Name = community.Name,
                District = community.District,
                State = community.State,
                ApproximatePopulation = community.ApproximatePopulation,
                PopulationReferenceYear = community.PopulationReferenceYear,
                Latitude = community.Latitude,
                Longitude = community.Longitude,
                CreatedAtUtc = community.CreatedAtUtc
            })
            .ToListAsync();

        var hasMore = communities.Count > PageSize;

        if (hasMore)
            communities.RemoveAt(PageSize);

        // Directory statistics.
        var activeCount = await _context.Communities
            .CountAsync(community => !community.IsArchived);

        var archivedCount = User.IsInRole("Administrator")
            ? await _context.Communities.CountAsync(c => c.IsArchived)
            : 0;

        var model = new CommunityDirectoryViewModel
        {
            Communities = communities,

            SearchTerm = search,
            SelectedState = state,
            SelectedDistrict = district,
            MinimumPopulation = minPopulation,
            MaximumPopulation = maxPopulation,
            ShowArchived = archived,

            SortBy = sortBy,
            SortDescending = descending,

            CurrentPage = page,
            PageSize = PageSize,
            HasMore = hasMore,

            TotalCommunities = totalCommunities,
            ActiveCommunities = activeCount,
            ArchivedCommunities = archivedCount,

            AvailableStates = await _context.Communities
                .AsNoTracking()
                .Where(c => !c.IsArchived)
                .Select(c => c.State)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync(),

            AvailableDistricts = await _context.Communities
                .AsNoTracking()
                .Where(c => !c.IsArchived)
                .Select(c => c.District)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync()
        };

        return View(model);
    }


    // GET: /Communities/Communities/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CommunityCreateViewModel());
    }

    // POST: /Communities/Communities/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CommunityCreateViewModel model)
    {
        // Coordinates must either both be provided or both be empty.
        if (model.Latitude.HasValue != model.Longitude.HasValue)
        {
            ModelState.AddModelError(
                string.Empty,
                "Please provide both latitude and longitude, or leave both empty."
            );
        }

        if (!ModelState.IsValid)
            return View(model);

        var community = new Community
        {
            Name = model.Name.Trim(),
            District = model.District.Trim(),
            State = model.State.Trim(),
            Description = string.IsNullOrWhiteSpace(model.Description)
                ? null
                : model.Description.Trim(),

            Latitude = model.Latitude,
            Longitude = model.Longitude,

            ApproximatePopulation = model.ApproximatePopulation,
            PopulationReferenceYear = model.PopulationReferenceYear,

            IsArchived = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Communities.Add(community);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"Community '{community.Name}' registered successfully.";

        return RedirectToAction(nameof(Index));
    }


    // GET: /Communities/Communities/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var community = await _context.Communities
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CommunityDetailsViewModel
            {
                Id = c.Id,
                Name = c.Name,
                District = c.District,
                State = c.State,
                Description = c.Description,

                Latitude = c.Latitude,
                Longitude = c.Longitude,

                ApproximatePopulation = c.ApproximatePopulation,
                PopulationReferenceYear = c.PopulationReferenceYear,

                CreatedAtUtc = c.CreatedAtUtc,

                TotalWaterSources = c.WaterSources.Count(),
                ActiveWaterSources = c.WaterSources.Count(w => w.IsActive),

                TotalHealthReports = c.HealthReports.Count(),
                TotalReportedCases = c.HealthReports.Sum(
                    h => (int?)h.ReportCount) ?? 0,

                TotalAlerts = c.Alerts.Count(),

                ActiveAlerts = c.Alerts.Count(a =>
                    a.Status == AlertStatus.New ||
                    a.Status == AlertStatus.UnderReview ||
                    a.Status == AlertStatus.Confirmed),

                WaterSources = c.WaterSources
                    .OrderBy(w => w.Name)
                    .Select(w => new WaterSourceDetailsViewModel
                    {
                        Id = w.Id,
                        Name = w.Name,
                        SourceType = w.SourceType,
                        IsActive = w.IsActive,
                        Latitude = w.Latitude,
                        Longitude = w.Longitude,
                        ObservationCount = w.Observations.Count(),
                        LastObservedAtUtc = w.Observations
                            .Select(o => (DateTime?)o.ObservedAtUtc)
                            .Max()
                    })
                    .ToList(),

                HealthReports = c.HealthReports
                    .OrderByDescending(h => h.ReportedAtUtc)
                    .Select(h => new HealthReportDetailsViewModel
                    {
                        Id = h.Id,
                        Category = h.Category,
                        ReportCount = h.ReportCount,
                        ReportedAtUtc = h.ReportedAtUtc,
                        Source = h.Source,
                        IsVerified = h.IsVerified
                    })
                    .ToList(),

                Alerts = c.Alerts
                    .OrderByDescending(a => a.CreatedAtUtc)
                    .Select(a => new AlertDetailsViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        Severity = a.Severity.ToString(),
                        Status = a.Status.ToString(),
                        CreatedAtUtc = a.CreatedAtUtc
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (community == null)
            return NotFound();

        if (await _context.Communities
                .AnyAsync(c => c.Id == id && c.IsArchived) &&
            !User.IsInRole("Administrator"))
        {
            return NotFound();
        }

        return View(community);
    }

}
