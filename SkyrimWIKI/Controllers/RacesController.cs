using Microsoft.AspNetCore.Mvc;
using SkyrimWIKI.Models;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Controllers;

// Read only pages for the playable races.
[Route("races")]
public class RacesController : Controller
{
    private readonly IRepository<Race> _races;

    public RacesController(IRepository<Race> races)
    {
        _races = races;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View(_races.GetAll().OrderBy(r => r.Name).ToList());
    }

    [HttpGet("{id:int}")]
    public IActionResult Details(int id)
    {
        var race = _races.GetById(id);
        return race is null ? NotFound() : View(race);
    }
}
