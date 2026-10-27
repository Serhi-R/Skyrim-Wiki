import { useEffect, useState } from "react";
import {
  fetchLocations,
  fetchQuests,
  type LocationOption,
  type QuestPage,
  type QuestQuery,
} from "../api";

const STRINGS = {
  en: {
    search: "Search",
    location: "Location",
    allLocations: "All locations",
    level: "Level",
    sortBy: "Sort by",
    sortTitle: "Title",
    sortLevelAsc: "Level, low to high",
    sortLevelDesc: "Level, high to low",
    sortNewest: "Newest first",
    sortOldest: "Oldest first",
    sortLocation: "Location",
    reset: "Reset",
    found: "quests found",
    exportCsv: "Export CSV",
    loadError: "Could not load the quests. Please try again.",
    nothing: "Nothing matches this filter.",
    previous: "\u2190 Previous",
    next: "Next \u2192",
    page: "Page",
    loading: "...",
  },
  bg: {
    search: "Търсене",
    location: "Локация",
    allLocations: "Всички локации",
    level: "Ниво",
    sortBy: "Сортирай по",
    sortTitle: "Заглавие",
    sortLevelAsc: "Ниво, възходящо",
    sortLevelDesc: "Ниво, низходящо",
    sortNewest: "Най-нови първо",
    sortOldest: "Най-стари първо",
    sortLocation: "Локация",
    reset: "Изчисти",
    found: "намерени куеста",
    exportCsv: "Експортирай CSV",
    loadError: "Неуспешно зареждане на куестовете. Опитайте отново.",
    nothing: "Няма резултати за този филтър.",
    previous: "\u2190 Предишна",
    next: "Следваща \u2192",
    page: "Страница",
    loading: "...",
  },
} as const;

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
    term: params.get("term") ?? "",
    locationId: params.get("locationId") ?? "",
    minLevel: params.get("minLevel") ?? "",
    maxLevel: params.get("maxLevel") ?? "",
    sort: params.get("sort") ?? "title",
    page: Number(params.get("page") ?? "1") || 1,
  };
}

function toSearchParams(query: QuestQuery): string {
  const params = new URLSearchParams();
  if (query.term) params.set("term", query.term);
  if (query.locationId) params.set("locationId", query.locationId);
  if (query.minLevel) params.set("minLevel", query.minLevel);
  if (query.maxLevel) params.set("maxLevel", query.maxLevel);
  if (query.sort && query.sort !== "title") params.set("sort", query.sort);
  if (query.page > 1) params.set("page", String(query.page));
  return params.toString();
}

function getLang(): "en" | "bg" {
  const root = document.getElementById("quest-catalog");
  const value = root?.dataset.lang;
  return value === "bg" ? "bg" : "en";
}

export function QuestCatalog() {
  const [query, setQuery] = useState<QuestQuery>(readQuery);
  const [result, setResult] = useState<QuestPage | null>(null);
  const [locations, setLocations] = useState<LocationOption[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const t = STRINGS[getLang()];

  useEffect(() => {
    const controller = new AbortController();
    fetchLocations(controller.signal)
      .then(setLocations)
      .catch(() => setLocations([]));
    return () => controller.abort();
  }, []);

  useEffect(() => {
    const search = toSearchParams(query);
    const url = `${window.location.pathname}${search ? `?${search}` : ""}`;
    window.history.replaceState(null, "", url);

    // 300 ms debounce keeps the API quiet while the search box is typed in,
    // and the controller cancels a request that a newer keystroke replaced.
    const controller = new AbortController();
    setLoading(true);

    const timer = window.setTimeout(() => {
      fetchQuests(query, controller.signal)
        .then((data) => {
          setResult(data);
          setError(null);
          setLoading(false);
        })
        .catch((error: unknown) => {
          if (error instanceof DOMException && error.name === "AbortError") {
            return;
          }
          setError(t.loadError);
          setLoading(false);
        });
    }, 300);

    return () => {
      window.clearTimeout(timer);
      controller.abort();
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [query]);

  // Any filter change starts again from page 1.
  const update = (patch: Partial<QuestQuery>) =>
    setQuery((current) => ({ ...current, page: 1, ...patch }));

  return (
    <div>
      <div className="frame grid grid-cols-1 gap-3 p-4 sm:grid-cols-2 lg:grid-cols-6">
        <div className="lg:col-span-2">
          <label htmlFor="Term" className="label mb-1 block">{t.search}</label>
          <input
            id="Term"
            value={query.term}
            onChange={(event) => update({ term: event.target.value })}
            placeholder="Barrow..."
            className="input"
          />
        </div>

        <div>
          <label htmlFor="LocationId" className="label mb-1 block">{t.location}</label>
          <select
            id="LocationId"
            value={query.locationId}
            onChange={(event) => update({ locationId: event.target.value })}
            className="input"
          >
            <option value="">{t.allLocations}</option>
            {locations.map((location) => (
              <option key={location.id} value={location.id}>
                {location.name}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className="label mb-1 block">{t.level}</label>
          <div className="flex items-center gap-2">
            <input
              type="number"
              min={1}
              max={81}
              value={query.minLevel}
              onChange={(event) => update({ minLevel: event.target.value })}
              placeholder="1"
              className="input min-w-0"
            />
            <span className="text-dust">-</span>
            <input
              type="number"
              min={1}
              max={81}
              value={query.maxLevel}
              onChange={(event) => update({ maxLevel: event.target.value })}
              placeholder="81"
              className="input min-w-0"
            />
          </div>
        </div>

        <div>
          <label htmlFor="Sort" className="label mb-1 block">{t.sortBy}</label>
          <select
            id="Sort"
            value={query.sort}
            onChange={(event) => update({ sort: event.target.value })}
            className="input"
          >
            <option value="title">{t.sortTitle}</option>
            <option value="level">{t.sortLevelAsc}</option>
            <option value="level_desc">{t.sortLevelDesc}</option>
            <option value="newest">{t.sortNewest}</option>
            <option value="oldest">{t.sortOldest}</option>
            <option value="location">{t.sortLocation}</option>
          </select>
        </div>

        <div className="flex items-end">
          <button type="button" onClick={() => setQuery(EMPTY_QUERY)} className="btn btn-ghost btn-block">
            {t.reset}
          </button>
        </div>
      </div>

      <div className="mt-4 flex flex-wrap items-center justify-between gap-3">
        <p className="flex items-center gap-2 text-sm text-dust">
          {result ? result.totalCount : 0} {t.found}
          {loading && <span className="text-xs text-dust/70">{t.loading}</span>}
        </p>

        <a href={`/quests/export?${toSearchParams(query)}`} className="btn btn-ghost btn-sm">
          {t.exportCsv}
        </a>
      </div>

      {error && <p className="note-error mt-3">{error}</p>}

      {result && result.items.length === 0 && !loading && (
        <p className="empty mt-6">{t.nothing}</p>
      )}

      {result && result.items.length > 0 && (
        <div className="mt-3 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {result.items.map((quest) => (
            <div key={quest.id} className="frame frame-hover h-full p-5">
              <div className="flex items-start justify-between gap-2">
                <h3 className="break-words font-display font-semibold text-ivory">
                  <a href={`/quests/${quest.id}`} className="hover:text-candle">
                    {quest.title}
                  </a>
                </h3>
                <span className="badge shrink-0">Lv. {quest.level}</span>
              </div>
              <p className="meta mt-2">{quest.locationName}</p>
              <p className="mt-2 text-sm leading-relaxed text-parchment/85">{quest.excerpt}</p>
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
            className="btn btn-ghost btn-sm"
          >
            {t.previous}
          </button>
          <span className="px-3 text-sm text-dust">
            {t.page} {result.page} / {result.totalPages}
          </span>
          <button
            type="button"
            disabled={query.page >= result.totalPages}
            onClick={() => setQuery((current) => ({ ...current, page: current.page + 1 }))}
            className="btn btn-ghost btn-sm"
          >
            {t.next}
          </button>
        </nav>
      )}
    </div>
  );
}
