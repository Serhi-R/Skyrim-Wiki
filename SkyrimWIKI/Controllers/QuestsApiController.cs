using Microsoft.AspNetCore.Mvc;
using SkyrimWIKI.Models.ViewModels;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Controllers;

// JSON endpoint behind the React quest catalog. The filtering and paging is
// the same QueryRepository.Search the ASP.NET page used before.
[ApiController]
[Route("api/quests")]
public class QuestsApiController : ControllerBase
{
    private readonly IQuestRepository _quests;

    public QuestsApiController(IQuestRepository quests)
    {
        _quests = quests;
    }

    [HttpGet]
    public IActionResult Search([FromQuery] QuestFilter filter)
    {
        var result = _quests.Search(filter);

        return Ok(new
        {
            items = result.Items.Select(QuestCardViewModel.From).ToList(),
            totalCount = result.TotalCount,
            page = result.Page,
            pageSize = result.PageSize,
            totalPages = result.TotalPages
        });
    }
}
