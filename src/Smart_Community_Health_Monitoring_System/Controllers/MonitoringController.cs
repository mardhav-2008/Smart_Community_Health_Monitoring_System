
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Community_Health_Monitoring_System.Data;
using Smart_Community_Health_Monitoring_System.Models;
using Smart_Community_Health_Monitoring_System.Models.Entities;

namespace Smart_Community_Health_Monitoring_System.Controllers;

public class MonitoringController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public MonitoringController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var activeAlertStatuses = new[]
        {
            AlertStatus.New,
            AlertStatus.UnderReview,
            AlertStatus.Confirmed
        };

        var model = new MonitoringDashboardViewModel
        {
            ProjectStatus = _environment.IsDevelopment()
                ? "DEVELOPMENT DEMO"
                : "DATABASE CONNECTED",

            LastUpdated = DateTime.UtcNow,

            // Count active water sources.
            MonitoredWaterSources = await _context.WaterSources
                .CountAsync(source => source.IsActive),

            // Sum aggregated community-level reports.
            CommunityReports = await _context.HealthReports
                .SumAsync(report => (int?)report.ReportCount) ?? 0,

            // Exclude dismissed and resolved alerts.
            ActiveAlerts = await _context.EarlyWarningAlerts
                .CountAsync(alert =>
                    activeAlertStatuses.Contains(alert.Status)),

            // Fetch the six most recent water observations.
            WaterObservations = await _context.WaterObservations
                .AsNoTracking()
                .OrderByDescending(observation => observation.ObservedAtUtc)
                .Take(6)
                .Select(observation => new Models.WaterObservation
                {
                    Location = observation.WaterSource.Name,
                    Parameter = observation.Parameter,
                    Reading = observation.Value.ToString(),
                    Unit = observation.Unit,
                    Status = observation.IsValidated
                        ? "Validated"
                        : "Pending"
                })
                .ToListAsync(),

            // Aggregate health reports by category.
            HealthSummaries = await _context.HealthReports
                .AsNoTracking()
                .GroupBy(report => report.Category)
                .Select(group => new HealthSummary
                {
                    Category = group.Key,
                    Reports = group.Sum(report => report.ReportCount)
                })
                .OrderByDescending(summary => summary.Reports)
                .ToListAsync()
        };

        return View(model);
    }
}
