using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Community_Health_Monitoring_System.Data;
using Smart_Community_Health_Monitoring_System.Models.Entities;

namespace Smart_Community_Health_Monitoring_System.Areas.Monitoring.Controllers;

[Area("Monitoring")]
[Authorize(Policy = "FieldWorkerOrAdministrator")]
public class WaterSourcesController(ApplicationDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await db.WaterSources.AsNoTracking()
            .Include(x => x.Community)
            .OrderBy(x => x.Community.Name).ThenBy(x => x.Name)
            .ToListAsync();
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCommunities();
        return View(new WaterSource());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WaterSource model)
    {
        model.Name = model.Name?.Trim() ?? "";
        model.SourceType = model.SourceType?.Trim() ?? "";
        if (model.Latitude.HasValue != model.Longitude.HasValue)
            ModelState.AddModelError("", "Enter both coordinates or leave both blank.");
        if (!await db.Communities.AnyAsync(c => c.Id == model.CommunityId && !c.IsArchived))
            ModelState.AddModelError(nameof(model.CommunityId), "Select an active community.");

        if (!ModelState.IsValid)
        {
            await LoadCommunities();
            return View(model);
        }

        model.IsActive = true;
        db.WaterSources.Add(model);
        await db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Water source registered.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var source = await db.WaterSources.AsNoTracking()
            .Include(x => x.Community)
            .Include(x => x.Observations)
            .FirstOrDefaultAsync(x => x.Id == id);
        return source is null ? NotFound() : View(source);
    }

    [HttpGet]
    public async Task<IActionResult> RecordObservation(int id)
    {
        var exists = await db.WaterSources.AnyAsync(x => x.Id == id && x.IsActive);
        if (!exists) return NotFound();
        return View(new WaterObservation { WaterSourceId = id, ObservedAtUtc = DateTime.UtcNow });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordObservation(WaterObservation model)
    {
        var source = await db.WaterSources.FirstOrDefaultAsync(x => x.Id == model.WaterSourceId && x.IsActive);
        if (source is null) return NotFound();
        model.Parameter = model.Parameter?.Trim() ?? "";
        model.Unit = model.Unit?.Trim() ?? "";
        if (!ModelState.IsValid) return View(model);
        model.RecordedAtUtc = DateTime.UtcNow;
        model.IsValidated = false;
        db.WaterObservations.Add(model);
        await db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Observation recorded and marked as unvalidated.";
        return RedirectToAction(nameof(Details), new { id = model.WaterSourceId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var item = await db.WaterSources.FindAsync(id);
        if (item is null) return NotFound();
        item.IsActive = !item.IsActive;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCommunities() =>
        ViewBag.Communities = new SelectList(
            await db.Communities.AsNoTracking().Where(c => !c.IsArchived)
                .OrderBy(c => c.Name).Select(c => new { c.Id, c.Name }).ToListAsync(),
            "Id", "Name");
}
