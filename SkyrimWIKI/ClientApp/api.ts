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

export async function fetchLocations(): Promise<LocationOption[]> {
  const response = await fetch("/api/locations");
  if (!response.ok) {
    throw new Error("Could not load locations");
  }
  return (await response.json()) as LocationOption[];
}

export async function fetchQuests(query: QuestQuery): Promise<QuestPage> {
  const params = new URLSearchParams();
  if (query.term) params.set("Term", query.term);
  if (query.locationId) params.set("LocationId", query.locationId);
  if (query.minLevel) params.set("MinLevel", query.minLevel);
  if (query.maxLevel) params.set("MaxLevel", query.maxLevel);
  if (query.sort) params.set("Sort", query.sort);
  params.set("Page", String(query.page));

  const response = await fetch(`/api/quests?${params.toString()}`);
  if (!response.ok) {
    throw new Error("Could not load quests");
  }
  return (await response.json()) as QuestPage;
}

export async function fetchComments(questId: number): Promise<CommentsResponse> {
  const response = await fetch(`/api/comments?questId=${questId}`);
  if (!response.ok) {
    throw new Error("Could not load comments");
  }
  return (await response.json()) as CommentsResponse;
}

export async function createComment(input: CommentInput): Promise<CommentItem> {
  const response = await fetch("/api/comments", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      RequestVerificationToken: antiforgeryToken(),
    },
    body: JSON.stringify(input),
  });
  if (!response.ok) {
    throw new Error("Could not save the comment");
  }
  return (await response.json()) as CommentItem;
}

export async function deleteComment(id: number): Promise<void> {
  const response = await fetch(`/api/comments/${id}`, {
    method: "DELETE",
    headers: { RequestVerificationToken: antiforgeryToken() },
  });
  if (!response.ok) {
    throw new Error("Could not delete the comment");
  }
}

// The antiforgery token is rendered as a hidden input by the Razor page, and
// the API expects it back in a header because the widget posts JSON.
export function antiforgeryToken(): string {
  const input = document.querySelector<HTMLInputElement>(
    'input[name="__RequestVerificationToken"]',
  );
  return input?.value ?? "";
}
