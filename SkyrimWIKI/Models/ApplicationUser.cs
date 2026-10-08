using Microsoft.AspNetCore.Identity;

namespace SkyrimWIKI.Models;

// The application's user. Everything Identity needs (UserName, Email,
// PasswordHash, ...) comes from IdentityUser. Add profile properties below
// and create a migration, because the AspNetUsers table changes shape.
public class ApplicationUser : IdentityUser
{
}
