using System.Text;
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
[Route("quests")]
public class QuestsController : Controller
{
    private readonly IQuestRepository _quests;
    private readonly IRepository<Location> _locations;
    private readonly ImageStorage _images;
    private readonly ILogger<QuestsController> _logger;
    public QuestsController(
        IQuestRepository quests,
        IRepository<Location> locations,
        ImageStorage images,
        ILogger<QuestsController> logger)
    {
        _quests = quests;
        _locations = locations;
        _images = images;
        _logger = logger;
    }

    // The list itself is rendered by the React catalog from /api/quests; this
    // action only serves the page shell.
    [HttpGet("")]
    public IActionResult Index() => View();

    // Downloads the quests that match the current filter as a CSV file.
    [HttpGet("export")]
    public IActionResult Export(QuestFilter filter)
    {
        // The whole result is wanted, not one page of it.
        filter.Page = 1;
        filter.PageSize = int.MaxValue;

        var csv = new StringBuilder();
        csv.AppendLine("Title,Level,Reward,Location,CreatedAt,Description");

        var quests = _quests.Search(filter).Items;

        foreach (var quest in quests)
        {
            csv.Append(Escape(quest.Title)).Append(',')
                .Append(quest.Level).Append(',')
                .Append(Escape(quest.Reward)).Append(',')
                .Append(Escape(quest.Location?.Name ?? "")).Append(',')
                .Append(quest.CreatedAt.ToString("yyyy-MM-dd")).Append(',')
                .Append(Escape(quest.Description))
                .AppendLine();
        }

        // The UTF-8 byte order mark makes Excel read the file as UTF-8, so
        // Bulgarian text in the descriptions is not garbled.
        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv.ToString()))
            .ToArray();

        _logger.LogInformation(
            "Quest export of {Count} rows requested by {User}",
            quests.Count, User.Identity?.Name);

        return File(bytes, "text/csv", $"quests-{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    // Wraps a value in quotes and doubles the quotes inside it, so commas,
    // quotes and line breaks cannot break the columns.
    private static string Escape(string value)
        => $"\"{value.Replace("\"", "\"\"")}\"";

    [HttpGet("{id:int}")]
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

    [HttpGet("create")]
    [Authorize]
    public IActionResult Create()
    {
        var vm = new QuestFormViewModel
        {
            Quest = new Quest(),
            Locations = new SelectList(_locations.GetAll(), "Id", "Name")
        };
        return View(vm);
    }

    [HttpPost("create")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuestFormViewModel vm)
    {
        var quest = vm.Quest;
        ValidateImage(quest);

        if (!ModelState.IsValid)
        {
            vm.Locations = new SelectList(_locations.GetAll(), "Id", "Name", quest.LocationId);
            return View(vm);
        }

        if (quest.Image is { Length: > 0 })
        {
            quest.ImagePath = await _images.SaveAsync(quest.Image);
        }

        _quests.Add(quest);
        _quests.SaveChanges();

        _logger.LogInformation(
            "Quest {QuestId} '{Title}' created by {User}",
            quest.Id, quest.Title, User.Identity?.Name);

        return RedirectToAction(nameof(Details), new { id = quest.Id });
    }

    [HttpGet("{id:int}/edit")]
    [Authorize]
    public IActionResult Edit(int id)
    {
        var quest = _quests.GetById(id);
        if (quest is null)
        {
            return NotFound();
        }

        var vm = new QuestFormViewModel
        {
            Quest = quest,
            Locations = new SelectList(_locations.GetAll(), "Id", "Name", quest.LocationId)
        };
        return View(vm);
    }

    [HttpPost("{id:int}/edit")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, QuestFormViewModel vm)
    {
        var quest = vm.Quest;
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
            vm.Locations = new SelectList(_locations.GetAll(), "Id", "Name", quest.LocationId);
            return View(vm);
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

        _logger.LogInformation(
            "Quest {QuestId} '{Title}' edited by {User}",
            existing.Id, existing.Title, User.Identity?.Name);

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("{id:int}/delete")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var quest = _quests.GetById(id);
        return quest is null ? NotFound() : View(quest);
    }

    [HttpPost("{id:int}/delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var quest = _quests.GetById(id);
        if (quest is null)
        {
            _logger.LogWarning("Quest {QuestId} not found", id);
            return NotFound();
        }

        _images.Delete(quest.ImagePath);
        _quests.Remove(quest);
        _quests.SaveChanges();

        _logger.LogWarning(
            "Quest {QuestId} '{Title}' deleted by {User}",
            quest.Id, quest.Title, User.Identity?.Name);

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

}
