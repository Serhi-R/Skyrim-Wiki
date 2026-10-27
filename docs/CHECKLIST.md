# Project checklist and evidence

This file maps the coursework requirements and the hardening work to the code,
with the command that proves each item. Commands assume the working directory
is the `SkyrimWIKI` project folder and the app is running on
`http://127.0.0.1:5099`.

## Course requirements

| Requirement | Status | Where | Evidence |
| --- | --- | --- | --- |
| N = 3 dynamic pages | done | Home, Quests, Races, Locations (4) | `GET /`, `/quests`, `/races`, `/locations` return 200 with data from the database |
| E = 1 object with full CRUD | done | Quests | `GET/POST /quests/create`, `/quests/{id}/edit`, `/quests/{id}/delete` |
| L = Layout, partial and section | done | `Views/Shared/_Layout.cshtml`, `_QuestCard`, `_QuestForm`, `Scripts` section | layout renders header, nav and footer; views call `@section Scripts` |
| Validation in model and views | done | data annotations on `Models/*`, `asp-validation-for` in forms | submitting an empty form returns the form with field errors |
| Database through EF Core | done | `Data/AppDbContext.cs`, `Data/Migrations`, `Data/DomainSeeder.cs` | `dotnet ef migrations list`, tables seeded on first run |
| Error handling | done | `UseExceptionHandler`, `UseStatusCodePagesWithReExecute("/status")` | `GET /no/such/page` returns 404 with the styled `StatusPage` view |
| Bonus: Identity | done | `Areas/Identity`, `Program.cs` | register, login, logout, lockout |
| Bonus: React + TypeScript | done | `ClientApp/`, bundled to `wwwroot/js/app.js` | quest catalog and comment widget mount on `/quests` and `/quests/{id}` |
| Bonus: image upload | done | `Services/ImageStorage.cs` | jpg/png/webp up to 2 MB, magic bytes checked |
| Bonus: CSV export | done | `QuestsController.Export` | `GET /quests/export` returns `text/csv` |
| Bonus: search, filter, sort, paging | done | `Repositories/QuestRepository.Search` | `GET /api/quests?term=barrow` returns one item |
| Bonus: comments and ratings | done | `Controllers/CommentsController.cs` | `GET /api/comments?questId=1` returns count and average |

## Security

| Item | Status | Where | Evidence |
| --- | --- | --- | --- |
| Admin password out of source | done | `appsettings.json`, user-secrets | `Identity:Admin:Password` set with `dotnet user-secrets` |
| Hardening headers | done | `Middleware/SecurityHeadersMiddleware.cs` | see the header dump below |
| Content security policy | done | same file | `default-src 'self'`, `script-src 'self'`, `font-src 'self'` |
| HTTPS and HSTS | done | `Program.cs` | `UseHttpsRedirection` always, `UseHsts` outside development |
| Password policy | done | `Program.cs` | 8 chars, upper, lower and a digit |
| Account lockout | done | `Program.cs` | 5 failed attempts lock for 5 minutes |
| Auth cookie flags | done | `Program.cs` | `HttpOnly`, `SameSite=Lax`, sliding expiration |
| Antiforgery on writes | done | controllers and `Program.cs` | header `RequestVerificationToken`; `[ValidateAntiForgeryToken]` on POST/DELETE |
| Upload validation | done | `Services/ImageStorage.cs` | size plus magic bytes, random file name, safe delete |
| Authorization on writes | done | `QuestsController`, `CommentsController` | `[Authorize]` on create, edit, delete and comment writes |

Header dump (`curl -sD - -o /dev/null http://127.0.0.1:5099/`):

```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
Referrer-Policy: strict-origin-when-cross-origin
Permissions-Policy: camera=(), microphone=(), geolocation=()
Content-Security-Policy: default-src 'self'; img-src 'self' data:; font-src 'self'; style-src 'self' 'unsafe-inline'; script-src 'self'; connect-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'; object-src 'none'
```

## Network and routing

| Item | Status | Evidence |
| --- | --- | --- |
| Attribute routing, lowercase URLs | done | `/`, `/about`, `/quests`, `/quests/1`, `/races`, `/locations`, `/api/*` all return 200; `/Home/Index` returns 404 |
| One static file mechanism | done | every page emits the same `/css/site.css?v=hash`; the earlier split between a query-string version and a build-time fingerprinted path (which 404ed after a css rebuild) is gone |
| Single fetch module | done | `ClientApp/api.ts` is the only place that calls `fetch` |
| Debounce and cancel | done | `QuestCatalog.tsx` debounces 300 ms and aborts the previous request |
| api search | done | `GET /api/quests?term=barrow&sort=level&page=1` returns matching rows |

## Design

| Item | Status | Evidence |
| --- | --- | --- |
| Cold Nordic palette | done | body background `#0F1318`; tokens in `ClientApp/styles/site.css` |
| Self-hosted fonts | done | `wwwroot/fonts`, `@font-face`; `document.fonts.check` true for Cinzel and EB Garamond |
| No external requests | done | no off-origin requests when loading `/quests` |
| Licensed imagery | done | game screenshots under `wwwroot/img/game/`, Wikimedia fallbacks in `wwwroot/img/` |
| Original logo | done | `wwwroot/img/logo.svg` |
| English and Bulgarian | done | `Localization/strings.json` plus `content.bg.json`; `/language/{lang}` sets a cookie and every page renders in the chosen tongue |
| Signature hero | done | aurora hero on `/` |
| Responsive, no overflow | done | 0 px horizontal overflow at 360 and 1280 on all pages |

## Verification log

```
npm run build:css      # Tailwind CSS
npm run build:js       # React bundle
npm run typecheck      # tsc --noEmit, no errors
dotnet build           # 0 warnings, 0 errors
```

Route smoke test (expected codes in brackets):

```
/ 200   /about 200   /quests 200   /quests/1 200
/quests/create 302   /quests/1/edit 302   /quests/1/delete 302   (redirect to login)
/quests/export 200   /races 200   /races/1 200
/locations 200   /locations/1 200   /language/bg 302
/api/quests 200   /api/locations 200   /api/comments?questId=1 200
/status?code=404 404   /Home/Index 404
```

### Security test results

| Test | Command | Result |
| --- | --- | --- |
| CSRF blocked on form POST | authenticated `POST /quests/create` with no token | 400 (fixed, see below) |
| CSRF blocked on API | `POST /api/comments` with no token | 400 `application/problem+json` |
| Unauthenticated write | `POST /quests/create` with no session | 302 to login |
| Fake image rejected | `POST /quests/create` with a text file named `evil.jpg` | form re-renders with "Choose a jpg, png or webp image up to 2 MB." |
| SQL injection in search | `GET /api/quests?term=' OR '1'='1` | `totalCount: 0`, EF Core parameterises the query |
| Production error handling | `ASPNETCORE_ENVIRONMENT=Production`, `GET /nope` | 404 with the styled status page, no stack trace |

Fix found during testing: a form POST that failed antiforgery validation
originally returned 405, because the status code pages middleware re executed
the request as `POST /status` and the status page is GET only. The re execution
is now limited to GET and HEAD, so the same request returns a correct 400.

## Known gaps

- Game screenshots are installed: 33 entities resolve to a game file under
  `wwwroot/img/game/` (1 hero, 10 races, 10 holds, 12 quests). `HoldImages.cs`
  keeps the older Wikimedia photos only as a fallback. See `docs/IMAGES-NEEDED.md`.
- The GSAP and ScrollTrigger motion layer is not added yet; the reduced-motion
  guard and the design token for it are in place.
- Quest levels and rewards in the seed data are rough guides, and a few details
  still need a pass against the reference pages; see `docs/CONTENT-GAPS.md`.
- Comment delete authorization is enforced in `CommentsController`
  (the signed-in user must own the comment), but a two-account ownership test
  has not been scripted yet.
- HSTS is configured with `UseHsts`, but it only sends the header over HTTPS;
  the local run is plain HTTP, so that header was not exercised.
