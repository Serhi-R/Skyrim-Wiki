using System.ComponentModel.DataAnnotations;

namespace SkyrimWIKI.Models;

// A playable race of the game. Shown on its own page; no relations yet.
public class Race : BaseEntity
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Playable")]
    public bool Playable { get; set; } = true;
}
