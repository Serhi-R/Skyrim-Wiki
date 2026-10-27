export interface QuestCard {
  id: number;
  title: string;
  level: number;
  locationName: string;
  excerpt: string;
}

export interface QuestPage {
  items: QuestCard[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface LocationOption {
  id: number;
  name: string;
}

export interface CommentItem {
  id: number;
  author: string;
  text: string;
  rating: number;
  createdAt: string;
  canDelete: boolean;
}

export interface CommentsResponse {
  questId: number;
  count: number;
  averageRating: number;
  items: CommentItem[];
}

export interface QuestQuery {
  term: string;
  locationId: string;
  minLevel: string;
  maxLevel: string;
  sort: string;
  page: number;
}

export interface CommentInput {
  questId: number;
  text: string;
  rating: number;
}

// One error type for every endpoint, so callers can branch on the status
// instead of parsing messages.
export class ApiError extends Error {
  constructor(
    public readonly status: number,
    message: string,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

const VERIFICATION_HEADER = "RequestVerificationToken";

interface RequestOptions {
  method?: string;
  body?: unknown;
  signal?: AbortSignal;
}

// The only place that calls fetch. It sets the antiforgery header on writes,
// unwraps JSON errors and returns a typed result.
async function request<T>(url: string, options: RequestOptions = {}): Promise<T> {
  const method = options.method ?? "GET";
  const headers: Record<string, string> = {};

  if (options.body !== undefined) {
    headers["Content-Type"] = "application/json";
  }
  if (method !== "GET") {
    headers[VERIFICATION_HEADER] = antiforgeryToken();
  }

  const response = await fetch(url, {
    method,
    headers,
    body: options.body !== undefined ? JSON.stringify(options.body) : undefined,
    signal: options.signal,
    credentials: "same-origin",
  });

  if (!response.ok) {
    throw new ApiError(response.status, await readMessage(response));
  }
  if (response.status === 204) {
    return undefined as T;
  }
  return (await response.json()) as T;
}

async function readMessage(response: Response): Promise<string> {
  try {
    const data = (await response.json()) as { message?: string; title?: string };
    return data.message ?? data.title ?? `Request failed (${response.status})`;
  } catch {
    return `Request failed (${response.status})`;
  }
}

export function fetchLocations(signal?: AbortSignal): Promise<LocationOption[]> {
  return request<LocationOption[]>("/api/locations", { signal });
}

export function fetchQuests(query: QuestQuery, signal?: AbortSignal): Promise<QuestPage> {
  const params = new URLSearchParams();
  if (query.term) params.set("term", query.term);
  if (query.locationId) params.set("locationId", query.locationId);
  if (query.minLevel) params.set("minLevel", query.minLevel);
  if (query.maxLevel) params.set("maxLevel", query.maxLevel);
  if (query.sort) params.set("sort", query.sort);
  params.set("page", String(query.page));

  return request<QuestPage>(`/api/quests?${params.toString()}`, { signal });
}

export function fetchComments(questId: number, signal?: AbortSignal): Promise<CommentsResponse> {
  return request<CommentsResponse>(`/api/comments?questId=${questId}`, { signal });
}

export function createComment(input: CommentInput): Promise<CommentItem> {
  return request<CommentItem>("/api/comments", { method: "POST", body: input });
}

export function deleteComment(id: number): Promise<void> {
  return request<void>(`/api/comments/${id}`, { method: "DELETE" });
}

// The antiforgery token is rendered as a hidden input by the Razor page, and
// the API expects it back in a header because the widget posts JSON.
export function antiforgeryToken(): string {
  const input = document.querySelector<HTMLInputElement>(
    'input[name="__RequestVerificationToken"]',
  );
  return input?.value ?? "";
}
