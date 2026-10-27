using Microsoft.AspNetCore.Mvc;
using SkyrimWIKI.Services;

namespace SkyrimWIKI.Controllers;

// Switches the reading language and returns the visitor to the page they were
// on. A server round trip keeps the choice in a cookie, so the correct text is
// rendered on the first paint (no flash of the other language).
[Route("language")]
public class LanguageController : Controller
{
    [HttpGet("{lang}")]
    public IActionResult Set(string lang, string? returnUrl)
    {
        var value = lang == "bg" ? "bg" : "en";
        Response.Cookies.Append(Localizer.CookieName, value, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });

        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
    }
}
