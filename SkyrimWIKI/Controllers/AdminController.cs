using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyrimWIKI.Data;
using SkyrimWIKI.Models;

namespace SkyrimWIKI.Controllers;

[Route("admin")]
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _db;

    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var items = new List<AdminUserViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            items.Add(new AdminUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? "",
                Roles = roles
            });
        }
        return View(items);
    }

    [HttpPost("promote")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Promote(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();
        if (!await _roleManager.RoleExistsAsync("Admin"))
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        if (!await _userManager.IsInRoleAsync(user, "Admin"))
            await _userManager.AddToRoleAsync(user, "Admin");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("demote")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Demote(string id)
    {
        if (id == _userManager.GetUserId(User)) return RedirectToAction(nameof(Index));
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();
        if (await _userManager.IsInRoleAsync(user, "Admin"))
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        return RedirectToAction(nameof(Index));
    }
}

public class AdminUserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
}
