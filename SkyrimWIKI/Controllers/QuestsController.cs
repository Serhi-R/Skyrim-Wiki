using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkyrimWIKI.Models;
using SkyrimWIKI.Models.ViewModels;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Controllers;

// Quest entries: list, details and the write actions. Reads go through the
// repositories; the date and author fields are always set here, never bound
// from the form.
public class QuestsController : Controller
{
    private readonly IRepository<Quest> _quests;
    private readonly IRepository<Location> _locations;

    public QuestsController(IRepository<Quest> quests, IRepository<Location> locations)
    {
        _quests = quests;
        _locations = locations;
    }

    public IActionResult Index()
    {
        var locations = _locations.GetAll().ToDictionary(l => l.Id);
        var cards = _quests.GetAll()
            .OrderBy(q => q.Title)
            .Select(q => QuestCardViewModel.From(
                q, locations.TryGetValue(q.LocationId, out var l) ? l.Name : ""))
            .ToList();

        return View(cards);
    }

    public IActionResult Details(int id)
    {
        var quest = _quests.GetById(id);
        if (quest is null)
        {
            return NotFound();
        }

        var location = _locations.GetById(quest.LocationId);

        var model = new QuestDetailsViewModel
        {
            Quest = quest,
            LocationName = location?.Name ?? "",
            LocationRegion = location?.Region ?? ""
        };

        return View(model);
    }

    [Authorize]
    public IActionResult Create()
    {
        LoadLocations();
        return View(new Quest());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Quest quest)
    {
        if (!ModelState.IsValid)
        {
            LoadLocations(quest.LocationId);
            return View(quest);
        }

        _quests.Add(quest);
        _quests.SaveChanges();

        return RedirectToAction(nameof(Details), new { id = quest.Id });
    }

    [Authorize]
    public IActionResult Edit(int id)
    {
        var quest = _quests.GetById(id);
        if (quest is null)
        {
            return NotFound();
        }

        LoadLocations(quest.LocationId);
        return View(quest);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Quest quest)
    {
        if (id != quest.Id)
        {
            return BadRequest();
        }

        var existing = _quests.GetById(id);
        if (existing is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            LoadLocations(quest.LocationId);
            return View(quest);
        }

        // Only editable fields are copied; the creation date is kept as it was.
        existing.Title = quest.Title;
        existing.Description = quest.Description;
        existing.Level = quest.Level;
        existing.Reward = quest.Reward;
        existing.LocationId = quest.LocationId;

        _quests.Update(existing);
        _quests.SaveChanges();

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize]
    public IActionResult Delete(int id)
    {
        var quest = _quests.GetById(id);
        return quest is null ? NotFound() : View(quest);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var quest = _quests.GetById(id);
        if (quest is null)
        {
            return NotFound();
        }

        _quests.Remove(quest);
        _quests.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    // Fills the location dropdown. The selected id is passed in on POST so the
    // choice survives a failed validation.
    private void LoadLocations(int? selectedId = null)
    {
        ViewData["Locations"] = new SelectList(_locations.GetAll(), "Id", "Name", selectedId);
    }
}
