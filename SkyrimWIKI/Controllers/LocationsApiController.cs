using Microsoft.AspNetCore.Mvc;
using SkyrimWIKI.Models;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Controllers;

// Small lookup endpoint: the location options of the quest filter dropdown.
[ApiController]
[Route("api/locations")]
public class LocationsApiController : ControllerBase
{
    private readonly IRepository<Location> _locations;

    public LocationsApiController(IRepository<Location> locations)
    {
        _locations = locations;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var items = _locations.GetAll()
            .OrderBy(l => l.Name)
            .Select(l => new { id = l.Id, name = l.Name })
            .ToList();

        return Ok(items);
    }
}
