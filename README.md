# SkyrimWIKI

A wiki about the quests, races and locations of Skyrim, built as the coursework
project for the Microsoft Web Technologies course at UniBIT.

The interface is available in English and Bulgarian, and the language choice is
remembered in the browser.

## Features

- Wiki pages for quests, races and locations
- Interactive quest catalog built with React: text search, location and level
  filters, six sort orders and paging, all kept in the URL
- Full CRUD for quests with validation on the model and in the views
- Image upload for quests (jpg, png or webp up to 2 MB)
- Registration, login and logout with ASP.NET Core Identity
- Comments with 1 to 5 star ratings, stored and read through a JSON API
- CSV export of the quest list, respecting the active filters
- Bilingual interface, English and Bulgarian
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

# 2. run the application
dotnet run
```

The database is created and seeded on the first start: migrations are applied
automatically, and the seeder adds races, locations, quests and a few comments
if the tables are still empty.

Default account created by the seeder, meant for local development only:

| Email | Password |
| --- | --- |
| admin@skyrimwiki.local | Admin123! |

The provider is chosen in `appsettings.json`. SQLite is the default; a
PostgreSQL connection string is included as an alternative.

## Project structure

```
SkyrimWIKI/
  Areas/Identity/   login, register and logout pages
  ClientApp/        React and TypeScript sources, bundled to wwwroot/js/app.js
  Controllers/      MVC controllers and the JSON API controllers
  Data/             DbContext, migrations and seeders
  Models/           entities, view models and API contracts
  Repositories/     the generic repository and the quest search query
  Services/         image storage
  Views/            Razor views, layouts and partials
  wwwroot/          compiled CSS and JS, libraries, uploaded images
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
