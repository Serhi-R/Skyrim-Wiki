using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SkyrimWIKI.Models;
using SkyrimWIKI.Models.ViewModels;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Controllers;

// Home renders the landing page and the project description. It only reads
// through the repository, like every other controller.
[Route("")]
public class HomeController : Controller
{
    private readonly IRepository<Quest> _quests;
    private readonly IRepository<Location> _locations;
    private readonly IRepository<Race> _races;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IRepository<Quest> quests,
        IRepository<Location> locations,
        IRepository<Race> races,
        ILogger<HomeController> logger)
    {
        _quests = quests;
        _locations = locations;
        _races = races;
        _logger = logger;
    }

    [HttpGet("")]
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

    [HttpGet("about")]
    public IActionResult About() => View();

    // Re executed for 404 and other error statuses. The status code is set
    // again because re executing resets it to 200. Named StatusPage so it does
    // not hide ControllerBase.StatusCode.
    [HttpGet("status")]
    public IActionResult StatusPage(int code)
    {
        Response.StatusCode = code;
        return View(code);
    }

    [HttpGet("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Unhandled error, request id {RequestId}", requestId);

        return View(new ErrorViewModel { RequestId = requestId });
    }
}
