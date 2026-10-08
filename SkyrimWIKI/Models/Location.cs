using System.ComponentModel.DataAnnotations;

namespace SkyrimWIKI.Models;

// A place in the game world. One location can hold many quests (1:N).
public class Location : BaseEntity
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Region")]
    public string Region { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    // Navigation property for the 1:N relation. EF Core fills this in when
    // related data is loaded; it is never set by hand.
    public List<Quest> Quests { get; set; } = [];
}
