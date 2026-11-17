using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Community_Health_Monitoring_System.Data;
using Smart_Community_Health_Monitoring_System.Models.Entities;

namespace Smart_Community_Health_Monitoring_System.Areas.Monitoring.Controllers;

[Area("Monitoring")]
[Authorize(Policy = "FieldWorkerOrAdministrator")]
public class AlertsController(ApplicationDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(AlertStatus? status = null)
    {
        var q = db.EarlyWarningAlerts.AsNoTracking().Include(a => a.Community).AsQueryable();
        if (status.HasValue) q = q.Where(a => a.Status == status.Value);
        ViewBag.Statuses = Enum.GetValues<AlertStatus>();
        ViewBag.SelectedStatus = status;
        return View(await q.OrderByDescending(a => a.CreatedAtUtc).Take(200).ToListAsync());
    }

    [Authorize(Policy = "AdministratorOnly")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, AlertStatus status)
    {
        var alert = await db.EarlyWarningAlerts.FindAsync(id);
        if (alert is null) return NotFound();
        alert.Status = status;
        if (status is AlertStatus.UnderReview or AlertStatus.Confirmed)
            alert.ReviewedAtUtc = DateTime.UtcNow;
        if (status is AlertStatus.Resolved or AlertStatus.Dismissed)
            alert.ResolvedAtUtc = DateTime.UtcNow;
        else
            alert.ResolvedAtUtc = null;
        await db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Alert status updated.";
        return RedirectToAction(nameof(Index));
    }

    // Demonstration signal: same community + same category >= 10 reports in rolling 7 days.
    // This is NOT a clinically validated outbreak threshold.
    [Authorize(Policy = "AdministratorOnly")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateFromRecentReports()
    {
        var since = DateTime.UtcNow.AddDays(-7);
        var signals = await db.HealthReports.AsNoTracking()
            .Where(r => r.ReportedAtUtc >= since && r.IsVerified)
            .GroupBy(r => new { r.CommunityId, r.Category })
            .Select(g => new { g.Key.CommunityId, g.Key.Category, Count = g.Sum(x => x.ReportCount) })
            .Where(x => x.Count >= 10)
            .ToListAsync();

        var created = 0;
        foreach (var signal in signals)
        {
            var exists = await db.EarlyWarningAlerts.AnyAsync(a =>
                a.CommunityId == signal.CommunityId &&
                a.Status != AlertStatus.Resolved &&
                a.Status != AlertStatus.Dismissed &&
                a.Title == $"Report cluster: {signal.Category}" &&
                a.CreatedAtUtc >= since);
            if (exists) continue;

            db.EarlyWarningAlerts.Add(new EarlyWarningAlert
            {
                CommunityId = signal.CommunityId,
                Title = $"Report cluster: {signal.Category}",
                Description = $"{signal.Count} verified reports were recorded in this category during the last 7 days. This is a review signal, not a diagnosis.",
                Severity = signal.Count >= 25 ? AlertSeverity.High : AlertSeverity.Moderate,
                Status = AlertStatus.New
            });
            created++;
        }
        await db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"{created} new review alert(s) generated.";
        return RedirectToAction(nameof(Index));
    }
}
