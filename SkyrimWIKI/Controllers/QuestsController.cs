using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkyrimWIKI.Models;
using SkyrimWIKI.Models.ViewModels;
using SkyrimWIKI.Repositories;
using SkyrimWIKI.Services;

namespace SkyrimWIKI.Controllers;

// Quest entries: list, details and the write actions. Reads go through the
// repositories; the date and author fields are always set here, never bound
// from the form.
public class QuestsController : Controller
{
    private readonly IQuestRepository _quests;
    private readonly IRepository<Location> _locations;
    private readonly IRepository<Comment> _comments;
    private readonly ImageStorage _images;

    public QuestsController(
        IQuestRepository quests,
        IRepository<Location> locations,
        IRepository<Comment> comments,
        ImageStorage images)
    {
        _quests = quests;
        _locations = locations;
        _comments = comments;
        _images = images;
    }

    public IActionResult Index(QuestFilter filter)
    {
        var result = _quests.Search(filter);

        var model = new QuestIndexViewModel
        {
            Filter = new QuestFilter
            {
                Term = filter.Term,
                LocationId = filter.LocationId,
                MinLevel = filter.MinLevel,
                MaxLevel = filter.MaxLevel,
                Sort = filter.Sort,
                Page = result.Page,
                PageSize = result.PageSize
            },
            Locations = _locations.GetAll().OrderBy(l => l.Name).ToList(),
            Quests = result.Items.Select(QuestCardViewModel.From).ToList(),
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages
        };

        return View(model);
    }

    public IActionResult Details(int id)
    {
        var quest = _quests.GetById(id);
        if (quest is null)
        {
            return NotFound();
        }

        var location = _locations.GetById(quest.LocationId);

        var comments = _comments.Find(c => c.QuestId == id)
            .OrderByDescending(c => c.CreatedAt)
            .ToList();

        var model = new QuestDetailsViewModel
        {
            Quest = quest,
            LocationName = location?.Name ?? "",
            LocationRegion = location?.Region ?? "",
            Comments = comments,
            AverageRating = comments.Count == 0 ? 0 : comments.Average(c => c.Rating)
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
    public async Task<IActionResult> Create(Quest quest)
    {
        ValidateImage(quest);

        if (!ModelState.IsValid)
        {
            LoadLocations(quest.LocationId);
            return View(quest);
        }

        if (quest.Image is { Length: > 0 })
        {
            quest.ImagePath = await _images.SaveAsync(quest.Image);
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
    public async Task<IActionResult> Edit(int id, Quest quest)
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

        ValidateImage(quest);

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

        if (quest.Image is { Length: > 0 })
        {
            // The old file is removed only after the new one is safely saved.
            var previousImage = existing.ImagePath;
            existing.ImagePath = await _images.SaveAsync(quest.Image);
            _images.Delete(previousImage);
        }

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

        _images.Delete(quest.ImagePath);
        _quests.Remove(quest);
        _quests.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    // The file is checked here because data annotations cannot express
    // "jpg, png or webp under 2 MB".
    private void ValidateImage(Quest quest)
    {
        if (quest.Image is { Length: > 0 } && !_images.IsAllowed(quest.Image))
        {
            ModelState.AddModelError(
                nameof(Quest.Image),
                "Choose a jpg, png or webp image up to 2 MB.");
        }
    }

    // Fills the location dropdown. The selected id is passed in on POST so the
    // choice survives a failed validation.
    private void LoadLocations(int? selectedId = null)
    {
        ViewData["Locations"] = new SelectList(_locations.GetAll(), "Id", "Name", selectedId);
    }
}
