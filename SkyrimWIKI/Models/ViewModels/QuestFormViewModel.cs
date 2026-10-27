using Microsoft.AspNetCore.Mvc.Rendering;
using SkyrimWIKI.Models;

namespace SkyrimWIKI.Models.ViewModels;

public class QuestFormViewModel
{
    public Quest Quest { get; set; } = new();
    public SelectList? Locations { get; set; }
}
