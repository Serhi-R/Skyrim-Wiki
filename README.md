# SkyrimWIKI

A wiki about the quests, races and locations of Skyrim, built as the coursework
project for the Microsoft Web Technologies course at UniBIT.

The interface is in English.

## Features

- Wiki pages for quests, races and locations
- Interactive quest catalog built with React: text search, location and level
  filters, six sort orders and paging, all kept in the URL
- Full CRUD for quests with validation on the model and in the views
- Image upload for quests (jpg, png or webp up to 2 MB, checked by magic bytes)
- Registration, login and logout with ASP.NET Core Identity, with account lockout
- Comments with 1 to 5 star ratings, stored and read through a JSON API
- CSV export of the quest list, respecting the active filters
- Responsive layout for phone, tablet and desktop, with a collapsible
  navigation menu on small screens
- Hardening response headers, a content security policy and HSTS in production
- Structured logging: console everywhere, rolling files only in development

## Requirement coverage

| Requirement | Where |
| --- | --- |
| N = 3 dynamic pages | Home, Quests, Races, Locations (four) |
| E = 1 object with full CRUD | Quests: list, details, create, edit, delete |
| L = 1 of Layout/Partial/Section | `_Layout.cshtml`, `_QuestCard`, `_QuestForm`, `Scripts` section |
| Validation in model and views | Data annotations on the models, `asp-validation-for` in the forms |
| Database through EF Core | SQLite with migrations and a seeder |
| Error handling | Exception handler and status code pages |

Bonus features: registration and login, React with TypeScript, image upload,
CSV export, 1 to N relations, search, filtering, sorting and pagination, and
comments with ratings.

## Routes and endpoints

Pages (attribute routed, lowercase URLs):

| Method | URL | Action | Auth |
| --- | --- | --- | --- |
| GET | `/` | Home landing page | no |
| GET | `/about` | Project description | no |
| GET | `/quests` | Quest catalog (React island) | no |
| GET | `/quests/{id}` | Quest details and comments | no |
| GET | `/quests/export` | CSV of the filtered list | no |
| GET | `/quests/create` | New quest form | yes |
| POST | `/quests/create` | Create a quest | yes |
| GET | `/quests/{id}/edit` | Edit form | yes |
| POST | `/quests/{id}/edit` | Update a quest | yes |
| GET | `/quests/{id}/delete` | Delete confirmation | yes |
| POST | `/quests/{id}/delete` | Delete a quest | yes |
| GET | `/races`, `/races/{id}` | Races list and details | no |
| GET | `/locations`, `/locations/{id}` | Locations list and details | no |
| GET | `/status?code={code}` | Re executed status page | no |
| GET | `/error` | Friendly error page | no |

Identity (Razor Pages):

| Method | URL |
| --- | --- |
| GET, POST | `/Identity/Account/Login` |
| GET, POST | `/Identity/Account/Register` |
| POST | `/Identity/Account/Logout` |
| GET | `/Identity/Account/AccessDenied` |
| GET | `/Identity/Account/Lockout` |

JSON API:

| Method | URL | Returns | Auth |
| --- | --- | --- | --- |
| GET | `/api/quests?term=&locationId=&minLevel=&maxLevel=&sort=&page=` | `{ items, totalCount, page, pageSize, totalPages }` | no |
| GET | `/api/locations` | `[{ id, name }]` | no |
| GET | `/api/comments?questId={id}` | `{ questId, count, averageRating, items }` | no |
| POST | `/api/comments` | 201 with the new comment; 400/404 on bad input | yes |
| DELETE | `/api/comments/{id}` | 204; 403 if not the author | yes |

Write requests to the API carry the antiforgery token in the
`RequestVerificationToken` header, because they post JSON and not a form.

## Technologies

- .NET 10, ASP.NET Core MVC with Razor views
- Entity Framework Core with SQLite
- ASP.NET Core Identity
- React 19 and TypeScript, bundled with esbuild
- Tailwind CSS 4
- Serilog

## Getting started

Prerequisites: the .NET SDK 10, Node.js 20 or newer, and the EF Core tools
(`dotnet tool install --global dotnet-ef`) if you want to manage migrations by
hand.

```bash
# 1. front end assets (Tailwind CSS and the React bundle)
npm install
npm run build

# 2. the seeded admin password, kept out of source control
dotnet user-secrets set "Identity:Admin:Password" "Admin123!"

# 3. run the application
dotnet run
```

The database is created and seeded on the first start: migrations are applied
automatically, and the seeder adds races, locations, quests and a few comments
if the tables are still empty.

The admin account is only created when both `Identity:Admin:Email` (in
`appsettings.json`) and `Identity:Admin:Password` (user-secrets or an
environment variable, for example `Identity__Admin__Password`) are set:

| Email | Password |
| --- | --- |
| admin@skyrimwiki.local | the value set with user-secrets |

The provider is chosen in `appsettings.json`. SQLite is the default; a
PostgreSQL connection string is included as an alternative.

## Security notes

- The admin password is never stored in `appsettings.json`; it comes from
  user-secrets in development and from environment variables in production.
- Every response carries `Content-Security-Policy`, `X-Content-Type-Options`,
  `X-Frame-Options`, `Referrer-Policy` and `Permissions-Policy` headers.
- The content security policy allows only same-origin scripts, styles, images
  and the self-hosted fonts.
- Passwords need 8 characters with upper and lower case letters and a digit;
  five failed attempts lock the account for five minutes.
- Uploads are limited to 2 MB and their magic bytes must match the extension.
- HSTS is enabled outside development, and the auth cookie is `HttpOnly` and
  `SameSite=Lax`.

## Project structure

```
SkyrimWIKI/
  Areas/Identity/   login, register, logout and error pages
  ClientApp/        React and TypeScript sources, bundled to wwwroot/js/app.js
  Controllers/      MVC controllers and the JSON API controllers
  Data/             DbContext, migrations and seeders
  Middleware/       security headers
  Models/           entities, view models and API contracts
  Repositories/     the generic repository and the quest search query
  Services/         image storage
  Views/            Razor views, layouts and partials
  wwwroot/          compiled CSS and JS, fonts, images, uploaded images
```

Controllers reach the database only through the repository pattern, so the
query logic lives in one place. The same `QuestRepository.Search` powers both
the React catalog and the CSV export.

## Development scripts

```bash
npm run build        # Tailwind CSS and the React bundle
npm run build:css    # Tailwind CSS only
npm run build:js     # React bundle only
npm run watch:css    # Tailwind CSS in watch mode
npm run typecheck    # TypeScript type check
```

Logging writes to the console in every environment and additionally to
`logs/dev-<date>.log` when the environment is Development.
