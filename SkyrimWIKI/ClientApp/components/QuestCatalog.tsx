import { useEffect, useState } from "react";
import {
  fetchLocations,
  fetchQuests,
  type LocationOption,
  type QuestPage,
  type QuestQuery,
} from "../api";

const EMPTY_QUERY: QuestQuery = {
  term: "",
  locationId: "",
  minLevel: "",
  maxLevel: "",
  sort: "title",
  page: 1,
};

// Reads the filter from the URL, so a filtered list can be bookmarked.
function readQuery(): QuestQuery {
  const params = new URLSearchParams(window.location.search);
  return {
    term: params.get("Term") ?? "",
    locationId: params.get("LocationId") ?? "",
    minLevel: params.get("MinLevel") ?? "",
    maxLevel: params.get("MaxLevel") ?? "",
    sort: params.get("Sort") ?? "title",
    page: Number(params.get("Page") ?? "1") || 1,
  };
}

function toSearchParams(query: QuestQuery): string {
  const params = new URLSearchParams();
  if (query.term) params.set("Term", query.term);
  if (query.locationId) params.set("LocationId", query.locationId);
  if (query.minLevel) params.set("MinLevel", query.minLevel);
  if (query.maxLevel) params.set("MaxLevel", query.maxLevel);
  if (query.sort && query.sort !== "title") params.set("Sort", query.sort);
  if (query.page > 1) params.set("Page", String(query.page));
  return params.toString();
}

export function QuestCatalog() {
  const [query, setQuery] = useState<QuestQuery>(readQuery);
  const [result, setResult] = useState<QuestPage | null>(null);
  const [locations, setLocations] = useState<LocationOption[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchLocations()
      .then(setLocations)
      .catch(() => setLocations([]));
  }, []);

  useEffect(() => {
    const search = toSearchParams(query);
    const url = `${window.location.pathname}${search ? `?${search}` : ""}`;
    window.history.replaceState(null, "", url);

    let cancelled = false;
    setLoading(true);

    // A short delay keeps the API quiet while the search box is typed in.
    const timer = window.setTimeout(() => {
      fetchQuests(query)
        .then((data) => {
          if (!cancelled) {
            setResult(data);
            setError(null);
          }
        })
        .catch(() => {
          if (!cancelled) {
            setError("Could not load the quests. Please try again.");
          }
        })
        .finally(() => {
          if (!cancelled) {
            setLoading(false);
          }
        });
    }, 250);

    return () => {
      cancelled = true;
      window.clearTimeout(timer);
    };
  }, [query]);

  // Any filter change starts again from page 1.
  const update = (patch: Partial<QuestQuery>) =>
    setQuery((current) => ({ ...current, page: 1, ...patch }));

  return (
    <div>
      <div className="grid grid-cols-1 gap-3 rounded-xl border border-slate-200 bg-white p-4 shadow-sm sm:grid-cols-2 lg:grid-cols-6">
        <div className="lg:col-span-2">
          <label htmlFor="Term" className="mb-1 block text-xs font-medium uppercase tracking-wide text-slate-400">
            <span className="t-en">Search</span>
            <span className="t-bg">Търсене</span>
          </label>
          <input
            id="Term"
            value={query.term}
            onChange={(event) => update({ term: event.target.value })}
            placeholder="Barrow..."
            className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
          />
        </div>

        <div>
          <label htmlFor="LocationId" className="mb-1 block text-xs font-medium uppercase tracking-wide text-slate-400">
            <span className="t-en">Location</span>
            <span className="t-bg">Локация</span>
          </label>
          <select
            id="LocationId"
            value={query.locationId}
            onChange={(event) => update({ locationId: event.target.value })}
            className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
          >
            <option value="">All / Всички</option>
            {locations.map((location) => (
              <option key={location.id} value={location.id}>
                {location.name}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className="mb-1 block text-xs font-medium uppercase tracking-wide text-slate-400">
            <span className="t-en">Level</span>
            <span className="t-bg">Ниво</span>
          </label>
          <div className="flex items-center gap-2">
            <input
              type="number"
              min={1}
              max={81}
              value={query.minLevel}
              onChange={(event) => update({ minLevel: event.target.value })}
              placeholder="1"
              className="w-full min-w-0 rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
            />
            <span className="text-slate-400">-</span>
            <input
              type="number"
              min={1}
              max={81}
              value={query.maxLevel}
              onChange={(event) => update({ maxLevel: event.target.value })}
              placeholder="81"
              className="w-full min-w-0 rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
            />
          </div>
        </div>

        <div>
          <label htmlFor="Sort" className="mb-1 block text-xs font-medium uppercase tracking-wide text-slate-400">
            <span className="t-en">Sort by</span>
            <span className="t-bg">Сортиране</span>
          </label>
          <select
            id="Sort"
            value={query.sort}
            onChange={(event) => update({ sort: event.target.value })}
            className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
          >
            <option value="title">Title / Заглавие</option>
            <option value="level">Level, low to high / Ниво, възходящо</option>
            <option value="level_desc">Level, high to low / Ниво, низходящо</option>
            <option value="newest">Newest first / Най-нови първо</option>
            <option value="oldest">Oldest first / Най-стари първо</option>
            <option value="location">Location / Локация</option>
          </select>
        </div>

        <div className="flex items-end">
          <button
            type="button"
            onClick={() => setQuery(EMPTY_QUERY)}
            className="w-full rounded-md bg-slate-100 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-200"
          >
            <span className="t-en">Reset</span>
            <span className="t-bg">Изчисти</span>
          </button>
        </div>
      </div>

      <div className="mt-4 flex flex-wrap items-center justify-between gap-3">
        <p className="flex items-center gap-2 text-sm text-slate-500">
          {result ? result.totalCount : 0}
          <span className="t-en">quests found</span>
          <span className="t-bg">намерени задания</span>
          {loading && <span className="text-xs text-slate-400">...</span>}
        </p>

        <a
          href={`/Quests/Export?${toSearchParams(query)}`}
          className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100"
        >
          <span className="t-en">Export CSV</span>
          <span className="t-bg">Експорт CSV</span>
        </a>
      </div>

      {error && (
        <p className="mt-3 rounded-md bg-rose-50 px-3 py-2 text-sm text-rose-700">{error}</p>
      )}

      {result && result.items.length === 0 && !loading && (
        <p className="mt-6 rounded-xl border border-dashed border-slate-300 p-8 text-center text-sm text-slate-500">
          <span className="t-en">Nothing matches this filter.</span>
          <span className="t-bg">Няма задания по този филтър.</span>
        </p>
      )}

      {result && result.items.length > 0 && (
        <div className="mt-3 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {result.items.map((quest) => (
            <div
              key={quest.id}
              className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm transition hover:shadow-md"
            >
              <div className="flex items-start justify-between gap-2">
                <h3 className="break-words font-semibold text-slate-900">
                  <a href={`/Quests/Details/${quest.id}`} className="hover:text-emerald-700">
                    {quest.title}
                  </a>
                </h3>
                <span className="shrink-0 rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-medium text-emerald-700">
                  Lv. {quest.level}
                </span>
              </div>
              <p className="mt-1 text-xs font-medium uppercase tracking-wide text-slate-400">
                {quest.locationName}
              </p>
              <p className="mt-2 text-sm leading-relaxed text-slate-600">{quest.excerpt}</p>
            </div>
          ))}
        </div>
      )}

      {result && result.totalPages > 1 && (
        <nav className="mt-6 flex flex-wrap items-center justify-center gap-2">
          <button
            type="button"
            disabled={query.page <= 1}
            onClick={() => setQuery((current) => ({ ...current, page: current.page - 1 }))}
            className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100 disabled:cursor-not-allowed disabled:opacity-40"
          >
            &larr; <span className="t-en">Previous</span>
            <span className="t-bg">Предишна</span>
          </button>
          <span className="px-3 text-sm text-slate-500">
            <span className="t-en">Page</span>
            <span className="t-bg">Страница</span> {result.page} / {result.totalPages}
          </span>
          <button
            type="button"
            disabled={query.page >= result.totalPages}
            onClick={() => setQuery((current) => ({ ...current, page: current.page + 1 }))}
            className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100 disabled:cursor-not-allowed disabled:opacity-40"
          >
            <span className="t-en">Next</span>
            <span className="t-bg">Следваща</span> &rarr;
          </button>
        </nav>
      )}
    </div>
  );
}
