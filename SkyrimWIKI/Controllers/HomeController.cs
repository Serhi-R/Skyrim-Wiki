using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SkyrimWIKI.Models;
using SkyrimWIKI.Models.ViewModels;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Controllers;

// Home renders the landing page and the project description. It only reads
// through the repository, like every other controller.
public class HomeController : Controller
{
    private readonly IRepository<Quest> _quests;
    private readonly IRepository<Location> _locations;
    private readonly IRepository<Race> _races;

    public HomeController(
        IRepository<Quest> quests,
        IRepository<Location> locations,
        IRepository<Race> races)
    {
        _quests = quests;
        _locations = locations;
        _races = races;
    }

    public IActionResult Index()
    {
        // The repository has no Include, so location names are joined in
        // memory here. The tables are small, and it keeps the query readable.
        var locations = _locations.GetAll().ToDictionary(l => l.Id);

        var model = new HomeViewModel
        {
            QuestCount = _quests.GetAll().Count(),
            LocationCount = locations.Count,
            RaceCount = _races.GetAll().Count(),
            LatestQuests = _quests.GetAll()
                .OrderByDescending(q => q.CreatedAt)
                .Take(3)
                .Select(q => QuestCardViewModel.From(
                    q, locations.TryGetValue(q.LocationId, out var l) ? l.Name : ""))
                .ToList()
        };

        return View(model);
    }

    public IActionResult About() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
