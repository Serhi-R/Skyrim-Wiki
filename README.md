# SkyrimWIKI

Курсов проект по Microsoft Web Technologies (UniBIT) - уики за куестове, раси и локации от Skyrim.

## Функционалности

- Страници за куестове, раси и локации
- Каталог с куестове на React + TypeScript - търсене, филтри по локация/ниво, сортиране, странициране, URL синхронизация
- Пълен CRUD за куестове с валидация в модела и изгледите
- Качване на изображения (jpg, png, webp, до 2 MB) с проверка по magic bytes
- Регистрация/вход/изход с ASP.NET Core Identity, заключване при грешни опити
- Коментари и оценки (1-5 звезди) през JSON API
- Експорт в CSV според активните филтри
- Административен панел - управление на потребители и роли
- Модерация - админ може да изтрива всякакви коментари и куестове
- Локализация EN/BG с превключвател
- Адаптивен дизайн за телефон/таблет/десктоп
- Сигурност - CSP, security headers, HSTS в продукция, HttpOnly/SameSite бисквитки

## Покритие на изискванията

| Изискване | Реализация |
| --- | --- |
| 3 динамични страници | Home, Quests, Races, Locations |
| Обект с пълен CRUD | Quests |
| Layout / Partial / Section | `_Layout`, паршъли, `@section Scripts` |
| Валидация в модел и изглед | DataAnnotations + `asp-validation-*` |
| База данни с EF Core | SQLite + миграции + seeders |
| Обработка на грешки | Exception handler + статус страници |

Бонуси: Identity, React + TypeScript, качване на изображения, CSV експорт, 1:N връзки, търсене/филтри/сортиране/странициране, коментари и рейтинги, роли и модерация, локализация.

## Рути

### Страници
| Метод | URL | Забележка |
| --- | --- | --- |
| GET | `/` | Начало |
| GET | `/about` | За проекта |
| GET | `/quests` | Каталог (React) |
| GET | `/quests/{id}` | Детайли + коментари |
| GET | `/quests/export` | CSV експорт |
| GET/POST | `/quests/create` | Създаване (само логнати) |
| GET/POST | `/quests/{id}/edit` | Редакция (само логнати) |
| GET/POST | `/quests/{id}/delete` | Изтриване (само Admin) |
| GET | `/races`, `/races/{id}` | Раси |
| GET | `/locations`, `/locations/{id}` | Локации |
| GET | `/admin` | Админ панел (само Admin) |
| GET | `/language/{lang}?returnUrl=` | Смяна на език |
| GET | `/status?code={code}`, `/error` | Грешки |

### Identity
| Метод | URL |
| --- | --- |
| GET/POST | `/Identity/Account/Login` |
| GET/POST | `/Identity/Account/Register` |
| POST | `/Identity/Account/Logout` |
| GET | `/Identity/Account/AccessDenied` |
| GET | `/Identity/Account/Lockout` |

### API
| Метод | URL | Бележка |
| --- | --- | --- |
| GET | `/api/quests?...` | Списък с филтри/сортиране/странициране |
| GET | `/api/locations` | Списък локации |
| GET | `/api/comments?questId={id}` | Коментари + средна оценка |
| POST | `/api/comments` | Добавяне (само логнати, с antiforgery) |
| DELETE | `/api/comments/{id}` | Изтриване (автор или Admin, с antiforgery) |

## Технологии

- .NET 10, ASP.NET Core MVC, Razor
- EF Core, SQLite
- ASP.NET Core Identity
- React 19 + TypeScript (esbuild)
- Tailwind CSS 4
- Serilog

## Старт

```bash
npm install
npm run build
dotnet user-secrets set "Identity:Admin:Password" "Admin123!"
dotnet run
```

Админ акаунт: `admin@skyrimwiki.local` (паролата от user-secrets). При първи старт се създават таблиците, миграциите и началните данни.

## Структура

```
SkyrimWIKI/
  Areas/Identity/   Login/Register/Logout
  ClientApp/        React + TS (компилира се в wwwroot/js/app.js)
  Controllers/      MVC + API контролери
  Data/             DbContext, миграции, seeders
  Localization/     EN/BG преводи
  Middleware/       Security headers, обфускация на полета
  Models/           Entities, ViewModels, API контракти
  Repositories/     Repository pattern + търсене
  Services/         ImageStorage, GameImages, Localizer
  Views/            Razor, Layout, Partials
  wwwroot/          CSS, JS, шрифтове, изображения
```

Контролерите работят през `IRepository<T>`. Логиката за търсене е в `QuestRepository` и се ползва от API и CSV експорта.
