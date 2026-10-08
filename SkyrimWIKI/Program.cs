using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkyrimWIKI.Data;
using SkyrimWIKI.Models;
using SkyrimWIKI.Repositories;

// Entry point: first services are registered, then the HTTP pipeline is built.
var builder = WebApplication.CreateBuilder(args);

// --- Database -------------------------------------------------------------
// SQLite by default; the provider is chosen in appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    DatabaseProviderSelector.Configure(options, builder.Configuration));

// --- Identity -------------------------------------------------------------
// Accounts, hashed passwords, cookies and sign-in. Password rules are relaxed
// for coursework; tighten them for a real application.
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// --- Repository -----------------------------------------------------------
// One open-generic registration: IRepository<Product> resolves to
// Repository<Product>. No line is needed per entity.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// --- MVC + Razor Pages ----------------------------------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Apply pending migrations and seed the database before the first request.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(services, app.Configuration);
    DomainSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    // Production shows a friendly error page instead of a stack trace.
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Authentication first ("who is the user?"), then authorization ("may they do
// this?").
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// /Quests/Edit/5 -> QuestsController.Edit(5).
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Identity screens: /Identity/Account/Login, Register, Logout.
app.MapRazorPages()
    .WithStaticAssets();

app.Run();
