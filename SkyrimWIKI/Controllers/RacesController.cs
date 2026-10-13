using Microsoft.AspNetCore.Mvc;
using SkyrimWIKI.Models;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Controllers;

// Read only pages for the playable races.
public class RacesController : Controller
{
    private readonly IRepository<Race> _races;

    public RacesController(IRepository<Race> races)
    {
        _races = races;
    }

    public IActionResult Index()
    {
        return View(_races.GetAll().OrderBy(r => r.Name).ToList());
    }

    public IActionResult Details(int id)
    {
        var race = _races.GetById(id);
        return race is null ? NotFound() : View(race);
    }
}
