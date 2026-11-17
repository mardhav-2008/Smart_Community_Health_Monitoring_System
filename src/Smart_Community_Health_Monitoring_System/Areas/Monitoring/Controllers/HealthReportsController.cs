using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Community_Health_Monitoring_System.Data;
using Smart_Community_Health_Monitoring_System.Models.Entities;

namespace Smart_Community_Health_Monitoring_System.Areas.Monitoring.Controllers;

[Area("Monitoring")]
[Authorize(Policy = "FieldWorkerOrAdministrator")]
public class HealthReportsController(ApplicationDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var q = db.HealthReports.AsNoTracking().Include(x => x.Community).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.Community.Name.Contains(search) || x.Category.Contains(search));
        return View(await q.OrderByDescending(x => x.ReportedAtUtc).Take(200).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCommunities();
        return View(new HealthReport { ReportedAtUtc = DateTime.UtcNow });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HealthReport model)
    {
        model.Category = model.Category?.Trim() ?? "";
        model.Source = string.IsNullOrWhiteSpace(model.Source) ? null : model.Source.Trim();
        if (model.ReportCount <= 0)
            ModelState.AddModelError(nameof(model.ReportCount), "Report count must be greater than zero.");
        if (!await db.Communities.AnyAsync(c => c.Id == model.CommunityId && !c.IsArchived))
            ModelState.AddModelError(nameof(model.CommunityId), "Select an active community.");

        if (!ModelState.IsValid)
        {
            await LoadCommunities();
            return View(model);
        }

        model.RecordedAtUtc = DateTime.UtcNow;
        model.IsVerified = false;
        db.HealthReports.Add(model);
        await db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Aggregated health report submitted for verification.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "AdministratorOnly")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(int id)
    {
        var report = await db.HealthReports.FindAsync(id);
        if (report is null) return NotFound();
        report.IsVerified = true;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCommunities() =>
        ViewBag.Communities = new SelectList(
            await db.Communities.AsNoTracking().Where(c => !c.IsArchived)
                .OrderBy(c => c.Name).Select(c => new { c.Id, c.Name }).ToListAsync(),
            "Id", "Name");
}
