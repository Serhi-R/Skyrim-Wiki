using Microsoft.AspNetCore.Mvc;
using SkyrimWIKI.Models;
using SkyrimWIKI.Models.ViewModels;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Controllers;

// Read only pages for the locations, including the quests found in each one.
public class LocationsController : Controller
{
    private readonly IRepository<Location> _locations;
    private readonly IRepository<Quest> _quests;

    public LocationsController(IRepository<Location> locations, IRepository<Quest> quests)
    {
        _locations = locations;
        _quests = quests;
    }

    public IActionResult Index()
    {
        // One query for all quests, then quests are counted per location in
        // memory. A query per location would be the classic N+1 problem.
        var questCounts = _quests.GetAll()
            .GroupBy(q => q.LocationId)
            .ToDictionary(g => g.Key, g => g.Count());

        var model = _locations.GetAll()
            .OrderBy(l => l.Name)
            .Select(l => new LocationCardViewModel
            {
                Location = l,
                QuestCount = questCounts.GetValueOrDefault(l.Id)
            })
            .ToList();

        return View(model);
    }

    public IActionResult Details(int id)
    {
        var location = _locations.GetById(id);
        if (location is null)
        {
            return NotFound();
        }

        var model = new LocationDetailsViewModel
        {
            Location = location,
            Quests = _quests.Find(q => q.LocationId == id)
                .OrderBy(q => q.Title)
                .Select(q => QuestCardViewModel.From(q, location.Name))
                .ToList()
        };

        return View(model);
    }
}
