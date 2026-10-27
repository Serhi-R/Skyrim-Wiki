using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SkyrimWIKI.Data;
using SkyrimWIKI.Middleware;
using SkyrimWIKI.Models;
using SkyrimWIKI.Repositories;
using SkyrimWIKI.Services;

// Entry point: first services are registered, then the HTTP pipeline is built.
var builder = WebApplication.CreateBuilder(args);

// --- Logging --------------------------------------------------------------
// Serilog writes to the console everywhere. A rolling log file is added only
// in development, so a deployed instance does not fill the disk and the
// development log is easy to find in logs/dev-<date>.log.
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();

    if (context.HostingEnvironment.IsDevelopment())
    {
        var logDirectory = Path.Combine(context.HostingEnvironment.ContentRootPath, "logs");
        configuration.WriteTo.File(
            Path.Combine(logDirectory, "dev-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7);
    }
});

// --- Database -------------------------------------------------------------
// SQLite by default; the provider is chosen in appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    DatabaseProviderSelector.Configure(options, builder.Configuration));

// --- Identity -------------------------------------------------------------
// Accounts, hashed passwords, cookies and sign-in. The admin password is not
// in appsettings.json: it is read from user-secrets in development and from
// environment variables in production.
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        // Five wrong passwords lock the account for five minutes, which slows
        // down online guessing without locking honest users out for long.
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Sign-in cookie: HttpOnly and SameSite=Lax so the antiforgery cookie and the
// auth cookie survive normal top level navigation but not cross-site posts.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// --- Repository -----------------------------------------------------------
// One open-generic registration: IRepository<Product> resolves to
// Repository<Product>. No line is needed per entity.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Entities with extra queries get their own registration: a request for
// IQuestRepository must resolve to QuestRepository, not to Repository<Quest>.
builder.Services.AddScoped<IQuestRepository, QuestRepository>();

// --- File uploads ---------------------------------------------------------
builder.Services.AddScoped<ImageStorage>();

// Resolves the picture for an entity: game drop first, then the fallback.
// Stateless and read-only, so one instance can be shared.
builder.Services.AddSingleton<GameImages>();

// --- Localization ---------------------------------------------------------
// The translation tables are loaded once; the per-request Localizer reads the
// chosen language from the "lang" cookie.
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<LocalizationStore>();
builder.Services.AddScoped<Localizer>();

// --- MVC + Razor Pages ----------------------------------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// The comment widget sends the antiforgery token in this header, because its
// POST goes out as JSON and not as a form.
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

var app = builder.Build();

// Hardening headers are attached to every response, including error pages.
app.UseMiddleware<SecurityHeadersMiddleware>();

// Apply pending migrations and seed the database before the first request.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(services, app.Configuration);
    DomainSeeder.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Production shows a friendly error page instead of a stack trace.
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 404s and other error statuses are re executed through the controller, so
// the user sees a real page inside the layout instead of an empty body.
// Re execution keeps the original HTTP method, and the status page is GET only,
// so this is limited to GET and HEAD. Without the guard a failed POST (for
// example a missing antiforgery token) would re execute as POST /status and the
// client would see 405 instead of the real 400.
app.UseWhen(
    context => HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method),
    branch => branch.UseStatusCodePagesWithReExecute("/status", "?code={0}"));

app.UseHttpsRedirection();

// One log entry per request: method, path, status code and duration.
app.UseSerilogRequestLogging();

// Every static file (css, js, images, fonts and uploads) is served here and the
// layout versions its links with asp-append-version. One mechanism, one URL,
// valid on every route: no build-time fingerprinted paths that go stale when
// the css is rebuilt without a full dotnet build.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        context.Context.Response.Headers.CacheControl = "public, max-age=604800";
    }
});

// Restore real Identity field names before model binding
app.UseFormFieldObfuscation();

app.UseRouting();

// Authentication first ("who is the user?"), then authorization ("may they do
// this?").
app.UseAuthentication();
app.UseAuthorization();

// Every MVC controller uses attribute routing, so no conventional route is
// registered. The controllers define lowercase URLs: /, /quests, /quests/5,
// /races, /locations and the /api/* endpoints.
app.MapControllers();

// Identity screens: /Identity/Account/Login, Register, Logout.
app.MapRazorPages();

app.Run();
